using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Runtime.Infrastructure.Runners;
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
        Func<NewAppDomainExecutor> executorFactory;
        private Project modelProject;


        public ModelMigrator(Project modelProject, Func<NewAppDomainExecutor> executorFactory)
        {
            this.modelProject = modelProject;
            this.executorFactory = executorFactory;

        }

        public void Migrate(IEnumerable<string> migrationIds, bool isRevert, bool force)
        {
            //TODO: vracet jiz hotove migrace v pripade chyby!
            foreach (var migrationId in migrationIds)
            {
                Migrate(migrationId, isRevert, force);
            }
        }


        //TODO: asi je zbytečný dělat každej krok v nové appdoméně - některé by to chtělo sloučit
        private void Migrate(string migrationId, bool isRevert, bool force)
        {
            HistoryTracker historyTracker = new HistoryTracker();
            string oldEdmxModel;
            ScaffoldedMigration scaffoldedMigration = null;

            try
            {
                oldEdmxModel = GetEdmxModel();

                //apply model changes
                using (var executor = executorFactory())
                {
                    executor.ExecuteRunner(new ApplyModelChangesRunner()
                    {
                        HistoryTracker = historyTracker,
                        ModelProject = modelProject,
                        ModelMigrationId = migrationId,
                        IsRevert = isRevert,
                        Force = force
                    });
                }



                //build project
                modelProject.Build(() => new ModelMigrationsException(Resources.CannotBuildProject));

                string newEdmxModel = GetEdmxModel();

                //generate db migration    
                using (var executor = executorFactory())
                {
                    scaffoldedMigration = executor.ExecuteRunner<ScaffoldedMigration>(new GenerateDbMigrationRunner()
                    {
                        ModelProject = modelProject,
                        ModelMigrationId = migrationId,
                        OldEdmxModel = oldEdmxModel,
                        NewEdmxModel = newEdmxModel,
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

        private string GetEdmxModel()
        {
            using (var executor = executorFactory())
            {
                return executor.ExecuteRunner<string>(new GetEdmxRunner());
            }
        }
    }
}
