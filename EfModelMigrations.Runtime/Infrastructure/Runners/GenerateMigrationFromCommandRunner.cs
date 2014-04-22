using EfModelMigrations.Commands;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Utilities;
using EfModelMigrations.Extensions;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Runtime.Infrastructure.Runners
{
    [Serializable]
    internal class GenerateMigrationFromCommandRunner : BaseRunner
    {
        public Project ModelProject { get; set; }

        public string CommandFullName { get; set; }
        public string MigrationName { get; set; }
        public bool IsRescaffold { get; set; }
        public object[] Parameters { get; set; }

        public override void Run()
        {
            var migrationsLocator = new ModelMigrationsLocator(Configuration);
            string rescaffoldMigrationId = null;
            //Check rescaffolding preconditions
            if (IsRescaffold)
            {
                var pendingMigrations = migrationsLocator.GetPendingMigrationsIds();
                if (pendingMigrations.Count() != 1)
                {
                    throw new ModelMigrationsException(Strings.Rescaffold_NoPendingMigration);
                }

                rescaffoldMigrationId = pendingMigrations.Single();
                string pendingMigrationName = ModelMigrationIdGenerator.GetNameFromId(rescaffoldMigrationId);
                if (!string.IsNullOrWhiteSpace(MigrationName) && !MigrationName.EqualsOrdinal(pendingMigrationName))
                {
                    throw new ModelMigrationsException(Strings.Rescaffold_SpecifiedNameNotSameAsPending(MigrationName, pendingMigrationName));
                }
                
                MigrationName = pendingMigrationName;
            }


            //Initialize Command
            ModelMigrationsCommand command;

            try
            {
                Type commandType = AppDomain.CurrentDomain.GetAssemblies()
                                        .SelectMany(a => 
                                                a.GetTypes().Where( t => t.FullName.EqualsOrdinal(CommandFullName)
                                            ))
                                        .Single();
                command = commandType.CreateInstance<ModelMigrationsCommand>(Parameters);
                command.MigrationName = this.MigrationName;
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(Strings.CannotCreateCommandInstance(CommandFullName), e);
            }

            
            var edmxModel = new EdmxModelExtractor().GetEdmxModelAsString(DbContext);
            var classModelProvider = new VsClassModelProvider(ModelProject, Configuration, EfModelMetadata.Load(edmxModel));
            var transformations = command.GetTransformations(classModelProvider);

            //params for generation
            string migrationName;
            string migrationId;
            if (IsRescaffold)
            {
                migrationName = MigrationName;
                migrationId = rescaffoldMigrationId;
            }
            else
            {
                migrationName = migrationsLocator.UniquifyMigrationName(command.GetMigrationName());
                migrationId = ModelMigrationIdGenerator.GenerateId(migrationName);
            }
            string migrationNamespace = Configuration.ModelMigrationsNamespace;

            var generator = Configuration.ModelMigrationGenerator;
            
            var generatedMigration = generator.GenerateMigration(migrationId, Configuration.ModelMigrationsDirectory, transformations, migrationNamespace, migrationName);
            
            Return(generatedMigration);
        }    
    }
}
