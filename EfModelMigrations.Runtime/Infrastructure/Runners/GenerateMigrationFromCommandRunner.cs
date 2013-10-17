using EfModelMigrations.Commands;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Utilities;
using EnvDTE;
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

        public Project ModelProject { get; set; }

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
            //TODO: vytvareni class model provideru je zde i v MigratorBaseRunner - nejak poresit
            var classModelProvider = new VsClassModelProvider(ModelProject, Configuration);
            var transformations = command.GetTransformations(classModelProvider);


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
