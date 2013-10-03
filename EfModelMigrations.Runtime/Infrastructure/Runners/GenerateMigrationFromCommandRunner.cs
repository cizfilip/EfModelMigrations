using EfModelMigrations.Commands;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners
{
    [Serializable]
    internal class GenerateMigrationFromCommandRunner : BaseRunner
    {
        public string CommandName { get; set; }
        public string[] Parameters { get; set; }

        public override void Run()
        {
            //Initialize Command
            //TODO: If Command not found Exception message from typefinder is too general, maybe rethrow here with better message
            var typeFinder = new TypeFinder();
            var commandType = typeFinder.FindType(typeof(ModelMigrationsCommand), GetConventionCommandName());
            ModelMigrationsCommand command = CreateInstance<ModelMigrationsCommand>(commandType);

            command.ParseParameters(Parameters);
            var transformations = command.GetTransformations();


            //params for generation
            //TODO: Migration name must be unique! We must check that...
            string migrationName = command.GetMigrationName();
            string migrationId = ModelMigrationIdGenerator.GenerateId(migrationName);
            string migrationNamespace = Configuration.ModelMigrationsNamespace;

            var generator = Configuration.ModelMigrationGenerator;
            
            var generatedMigration = generator.GenerateMigration(migrationId, transformations, migrationNamespace, migrationName);

            generatedMigration.MigrationId = migrationId;
            generatedMigration.MigrationDirectory = Configuration.ModelMigrationsDirectory;

            Return(generatedMigration);
        }

        private string GetConventionCommandName()
        {
            return CommandName + "Command";
        }

    

        
    }
}
