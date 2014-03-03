using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Runtime.Infrastructure.Runners.Migrators;
using EfModelMigrations.Runtime.Properties;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.IO;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    internal class ModelMigrator
    {
        private Func<NewAppDomainExecutor> executorFactory;
        private Project modelProject;

        public ModelMigrator(Project modelProject, Func<NewAppDomainExecutor> executorFactory)
        {
            this.executorFactory = executorFactory;
            this.modelProject = modelProject;
        }

        public void Migrate(IEnumerable<string> migrationIds, bool isRevert)
        {
            //TODO: vracet jiz hotove migrace v pripade chyby!
            foreach (var migrationId in migrationIds)
            {
                Migrate(migrationId, isRevert);
            }
        }


        //TODO: asi je zbytečný dělat každej krok v nové appdoméně - některé by to chtělo sloučit
        private void Migrate(string migrationId, bool isRevert)
        {
            HistoryTracker historyTracker = new HistoryTracker();
            string oldEdmxModel;
            ScaffoldedMigration scaffoldedMigration = null;

            try
            {
                //apply model changes
                using (var executor = executorFactory())
                {
                    oldEdmxModel = executor.ExecuteRunner<string>(new ApplyModelChangesRunner()
                    {
                        HistoryTracker = historyTracker,
                        ModelProject = modelProject,
                        ModelMigrationId = migrationId,
                        IsRevert = isRevert
                    });
                }


                //build project
                modelProject.Build(() => new ModelMigrationsException(Resources.CannotBuildProject));

                //generate db migration    
                using (var executor = executorFactory())
                {
                    scaffoldedMigration = executor.ExecuteRunner<ScaffoldedMigration>(new GenerateDbMigrationRunner()
                    {
                        ModelProject = modelProject,
                        ModelMigrationId = migrationId,
                        OldEdmxModel = oldEdmxModel,
                        IsRevert = isRevert
                    });
                }

                //write migration
                new DbMigrationWriter(modelProject).Write(scaffoldedMigration);


                //build project
                modelProject.Build(() => new ModelMigrationsException(Resources.CannotBuildProject));

                //update DB
                using (var executor = executorFactory())
                {
                    executor.ExecuteRunner(new UpdateDatabaseRunner()
                    {
                        ModelProject = modelProject,
                        ModelMigrationId = migrationId,
                        TargetDbMigration = scaffoldedMigration.MigrationId
                    });

                    //update applied migrations in configuration 
                    executor.ExecuteRunner(new UpdateConfigurationRunner()
                    {
                        //TODO: updatovat na projekt kde jsou migrace ne modelProject - az budu podporovat vicero projektu
                        MigrationProject = modelProject,
                        IsRevert = isRevert,
                        AppliedModelMigrationId = migrationId,
                        AppliedDbMigrationId = scaffoldedMigration.MigrationId
                    });
                }
            }
            //TODO: Pokud dojde k vyjimce v restore na historyTrackeru tak se vyhodi jen ta a my zapomeneme duvod proc jsme vubec restorovali
            //idealni vypis:
            // 1) message vyjimky ktera zpusobila nutnost restoru!
            // 2) pokud se restore provedl ok vypsat: vsechny zmeny pred chybou byli vraceny, pokud je vyjimka i v restoru napsat ze je to uplne v haji a at si uzivatel poradi sam :)
            catch (ModelMigrationsException)
            {
                RollbackModelState(historyTracker, scaffoldedMigration);
                throw;
            }
            catch (Exception e) 
            {
                RollbackModelState(historyTracker, scaffoldedMigration);
                throw new ModelMigrationsException(Resources.ApplyMigrationError, e);
            }
        }

        private void RollbackModelState(HistoryTracker historyTracker, ScaffoldedMigration scaffoldedMigration)
        {
            historyTracker.Restore(modelProject);

            //Ensure db migration is removed
            if (scaffoldedMigration != null)
            {
                new DbMigrationWriter(modelProject).RemoveMigration(scaffoldedMigration);
            }
        }
    }
}
