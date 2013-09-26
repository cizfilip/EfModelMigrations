using EfModelMigrations.Runtime.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Properties;
using EfModelMigrations.Runtime.Infrastructure.Runners.TypeFinders;
using EfModelMigrations.Runtime.Templates;
using System.IO;
using EfModelMigrations.Configuration;

namespace EfModelMigrations.Runtime.PowerShell
{
    class EnableCommand : PowerShellCommand
    {
        public EnableCommand(string[] parameters) : base(parameters)
        {
            Execute();
        }

        protected override void ExecuteCore()
        {
            
            using (var executor = CreateExecutor())
            {
                //find model migration configuration
                bool configurationExists = executor.ExecuteRunner<bool>(new FindModelMigrationsConfigurationRunner());
                if (configurationExists)
                {
                    WriteLine(Resources.ModelMigrationsAlreadyEnabled);
                    return;
                }

                //TODO: az budem prepirat migrationsdirectory pres parametr nesmi to byt absolutni cesta!! - check zde i v setteru v configuraci

                //create configuration
                string configurationFileName = "ModelMigrationsConfiguration.cs";
                string migrationsDirectory = ModelMigrationsConfigurationBase.DefaultModelMigrationsDirectory;
                string migrationsNamespace = Project.GetRootNamespace() + "." + migrationsDirectory;
                string configuration = new ModelMigrationsConfigurationTemplate() { Namespace = migrationsNamespace }.TransformText();

                Project.AddContentToProject(Path.Combine(migrationsDirectory, configurationFileName), configuration);


                //find Db migrations configuration
                bool dbConfigurationExists = executor.ExecuteRunner<bool>(new FindDbMigrationsConfigurationRunner());
                if (!dbConfigurationExists)
                {
                    //enable db migrations
                    //find db context
                    bool dbContextExists = executor.ExecuteRunner<bool>(new FindDbContextRunner());

                    if (!dbConfigurationExists)
                    {
                        //create db context
                        string contextName = Project.Name + "Context";
                        string contextFileName = contextName + ".cs";
                        string contextNamespace = Project.GetRootNamespace();
                        string context = new DbContextTemplate() { ContextName = contextName, Namespace = contextNamespace }.TransformText();

                        Project.AddContentToProject(contextFileName, context);
                    }

                    //call enable migrations
                    //InvokeScript("Enable-Migrations");

                }

            }

            WriteLine(Resources.ModelEnableSuccessfull);
        }
    }
}
