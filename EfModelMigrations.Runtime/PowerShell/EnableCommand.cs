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
            //build project
            var project = Project;

            if (!project.TryBuild())
            {
                throw new ModelMigrationsException(Resources.CannotBuildProject);
            }

            using (var executor = CreateExecutor())
            {
                //find model migration configuration
                bool configurationExists = executor.ExecuteRunner<bool>(new FindModelMigrationsConfigurationRunner());
                if (configurationExists)
                {
                    WriteLine(Resources.ModelMigrationsAlreadyEnabled);
                    return;
                }

                //create configuration
                string configurationFileName = "ModelMigrationsConfiguration.cs";
                string migrationsDirectory = "ModelMigrations";
                string migrationsNamespace = project.GetRootNamespace() + "." + migrationsDirectory;
                string configuration = new ModelMigrationsConfigurationTemplate().Init(migrationsNamespace).TransformText();

                project.AddContentToProject(Path.Combine(migrationsDirectory, configurationFileName), configuration);


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
                        string contextName = project.Name + "Context";
                        string contextFileName = contextName + ".cs";
                        string contextNamespace = project.GetRootNamespace();
                        string context = new DbContextTemplate().Init(contextNamespace, contextName).TransformText();

                        project.AddContentToProject(contextFileName, context);
                    }

                    //call enable migrations
                    //InvokeScript("Enable-Migrations");

                }

            }

            WriteLine(Resources.ModelEnableSuccessfull);
        }
    }
}
