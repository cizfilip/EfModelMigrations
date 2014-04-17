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
        public object[] Parameters { get; set; }

        public override void Run()
        {
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
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(Strings.CannotCreateCommandInstance(CommandFullName), e);
            }

            var edmxModel = new EdmxModelExtractor().GetEdmxModelAsString(DbContext);
            var classModelProvider = new VsClassModelProvider(ModelProject, Configuration, edmxModel);
            var transformations = command.GetTransformations(classModelProvider);

            //params for generation
            string migrationName = new ModelMigrationsLocator(Configuration).UniquifyMigrationName(command.GetMigrationName());
            string migrationId = ModelMigrationIdGenerator.GenerateId(migrationName);
            string migrationNamespace = Configuration.ModelMigrationsNamespace;

            var generator = Configuration.ModelMigrationGenerator;
            
            var generatedMigration = generator.GenerateMigration(migrationId, Configuration.ModelMigrationsDirectory, transformations, migrationNamespace, migrationName);
            
            Return(generatedMigration);
        }    
    }
}
