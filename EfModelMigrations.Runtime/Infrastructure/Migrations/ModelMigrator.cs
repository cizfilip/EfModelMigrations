using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Runtime.Infrastructure.Runners.Migrators;
using EfModelMigrations.Runtime.Properties;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;

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


        private void Migrate(string migrationId, bool isRevert)
        {
            //apply model changes
            using (var executor = executorFactory())
            {
                ExecuteApplyRunner(executor, migrationId, isRevert);
            }

            ScaffoldedMigration scaffoldedMigration = null;
            //generate db migration            
            try
            {
                //build project
                modelProject.Build(() => new ModelMigrationsException(Resources.CannotBuildProject));

                using (var executor = executorFactory())
                {
                    scaffoldedMigration = executor.ExecuteRunner<ScaffoldedMigration>(new GenerateDbMigrationRunner()
                    {
                        ModelProject = modelProject,
                        ModelMigrationId = migrationId,
                        IsRevert = isRevert
                    });
                }

                //write migration
                new DbMigrationWriter(modelProject).Write(scaffoldedMigration);

            }
            catch (Exception) //TODO: mozna jen ModelMigrationsException
            {
                //Ensure nothing from migration is added to project if error happen
                RemoveDbMigration(scaffoldedMigration);
                                
                RevertModelChanges(migrationId, isRevert);
                throw;
            }

            //update DB
            try
            {
                //build project
                modelProject.Build(() => new ModelMigrationsException(Resources.CannotBuildProject));

                using (var executor = executorFactory())
                {
                    executor.ExecuteRunner(new UpdateDatabaseRunner()
                    {
                        ModelProject = modelProject,
                        ModelMigrationId = migrationId,
                        TargetDbMigration = scaffoldedMigration.MigrationId
                    });
                }
            }
            catch (Exception)
            {
                RemoveDbMigration(scaffoldedMigration);
                RevertModelChanges(migrationId, isRevert);
                throw;
            }
        }

        private void ExecuteApplyRunner(NewAppDomainExecutor executor, string migrationId, bool isRevert)
        {
            executor.ExecuteRunner(new ApplyModelChangesRunner()
            {
                ModelProject = modelProject,
                ModelMigrationId = migrationId,
                IsRevert = isRevert
            });
        }

        private void RevertModelChanges(string migrationId, bool isRevert)
        {
            //Run apply changes in oposite direction
            using (var executor = executorFactory())
            {
                ExecuteApplyRunner(executor, migrationId, !isRevert);
            }
        }

        private void RemoveDbMigration(ScaffoldedMigration scaffoldedMigration)
        {
            if (scaffoldedMigration != null)
            {
                new DbMigrationWriter(modelProject).RemoveMigration(scaffoldedMigration);
            }
        }
    }
}
