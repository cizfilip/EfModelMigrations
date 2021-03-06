﻿using EfModelMigrations.Runtime.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Infrastructure.Runners.TypeFinders;
using EfModelMigrations.Runtime.Templates;
using System.IO;
using EfModelMigrations.Configuration;
using System.Resources;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Runtime.PowerShell
{
    class EnableCommand : PowerShellCommand
    {
        public EnableCommand()
            : base()
        {
            Execute();
        }

        //TODO: mel bych kontrolovat zarucit ze DB bude sqlserver pro jin0 db nem8m gener8tor sql pro migrace...
        protected override void ExecuteCore()
        {
            using (var facade = CreateFacade())
            {
                //find model migration configuration
                string configurationType = facade.FindModelMigrationsConfiguration();
                if (!string.IsNullOrEmpty(configurationType))
                {
                    WriteLine(Strings.ModelMigrationsAlreadyEnabled);
                    return;
                }
            }

            

            //create db context
            //TODO: musime zajistit ze DBContext (at jiz vytvorenz zde nebo pozdeji jiz existujici ma v sobe using na namespace modelu) - tak abz se zkompiloval kod napr. IDbSet<Person>
            string contextName = Project.Name + "Context";

            WriteLine(Strings.EnableCommand_CreatingDbContext(contextName));

            string contextFileName = contextName + ".cs";
            string contextNamespace = Project.GetRootNamespace();
            string context = new DbContextTemplate() { ContextName = contextName, Namespace = contextNamespace }.TransformText();

            Project.AddContentToProject(contextFileName, context);

            WriteLine(Strings.EnableCommand_EnablingEfMigrations);
            string migrationsDirectory = ModelMigrationsConfigurationBase.DefaultModelMigrationsDirectory;
            //Enable ef migrations
            string efMigrationsDirectory = "DbMigrations";
            string efMigrationsFullDirectory = Path.Combine(migrationsDirectory, efMigrationsDirectory);
            string efMigrationsConfigTypeName = efMigrationsDirectory + ".Configuration";
            InvokeScript("Enable-Migrations -MigrationsDirectory " + efMigrationsFullDirectory);

            WriteLine(Strings.EnableCommand_CreatingConfiguration);
            //create configuration
            string configurationFile = "ModelConfiguration";
            string configurationFileName = configurationFile + ".cs";
            string configurationFilePath = Path.Combine(migrationsDirectory, configurationFileName);

            string migrationsNamespace = Project.GetRootNamespace() + "." + migrationsDirectory;
            string configuration = new ModelMigrationsConfigurationTemplate()
            {
                Namespace = migrationsNamespace,
                ModelNamespace = Project.GetRootNamespace(),
                EfMigrationsConfigurationFullName = efMigrationsConfigTypeName
            }
                .TransformText();
            Project.AddContentToProject(configurationFilePath, configuration);
            //TODO: Zajistit aby byl resource soubor checknut do source control (je to jenom zamek pro TFS) - btw mozna zarucit pro vsechny nove pridane soubory!
            string resourceFileName = configurationFile + ".resx";
            string resourcePath = Path.Combine(migrationsDirectory, resourceFileName);
            WriteResourceFile(resourcePath);


            //TODO: zrevidovat TODOcka ze stare implementace Enablu:
            //using (var executor = CreateExecutor())
            //{
            //    //find model migration configuration
            //    bool configurationExists = executor.ExecuteRunner<bool>(new FindModelMigrationsConfigurationRunner());
            //    if (configurationExists)
            //    {
            //        WriteLine(Resources.ModelMigrationsAlreadyEnabled);
            //        return;
            //    }

            //    //TODO: az budem prepirat migrationsdirectory pres parametr nesmi to byt absolutni cesta!! - check zde i v setteru v configuraci
            //    //+ nastavit novou directory cestu v construktoru vygenerovane konfigurace

            //    //create configuration
            //    string configurationFile = "Configuration";
            //    string configurationFileName = configurationFile + ".cs";
            //    string migrationsDirectory = ModelMigrationsConfigurationBase.DefaultModelMigrationsDirectory;
            //    string migrationsNamespace = Project.GetRootNamespace() + "." + migrationsDirectory;
            //    string configuration = new ModelMigrationsConfigurationTemplate() 
            //        { 
            //            Namespace = migrationsNamespace,
            //            ModelNamespace = Project.GetRootNamespace(),
            //            EfMigrationsConfigurationFullName = ??
            //        }
            //        .TransformText();

            //    Project.AddContentToProject(Path.Combine(migrationsDirectory, configurationFileName), configuration);
            //    //TODO: Zajistit aby byl resource soubor checknut do source control (je to jenom zamek pro TFS) - btw mozna zarucit pro vsechny nove pridane soubory!
            //    string resourceFileName = configurationFile + ".resx";
            //    string resourcePath = Path.Combine(migrationsDirectory, resourceFileName);
            //    WriteResourceFile(resourcePath);

            //    //find Db migrations configuration
            //    bool dbConfigurationExists = executor.ExecuteRunner<bool>(new FindDbMigrationsConfigurationRunner());
            //    if (!dbConfigurationExists)
            //    {
            //        //enable db migrations
            //        //find db context
            //        bool dbContextExists = executor.ExecuteRunner<bool>(new FindDbContextRunner());

            //        if (!dbConfigurationExists)
            //        {
            //            //create db context
            //            //TODO: musime zajistit ze DBContext (at jiz vytvorenz zde nebo pozdeji jiz existujici ma v sobe using na namespace modelu) - tak abz se zkompiloval kod napr. IDbSet<Person>
            //            string contextName = Project.Name + "Context";
            //            string contextFileName = contextName + ".cs";
            //            string contextNamespace = Project.GetRootNamespace();
            //            string context = new DbContextTemplate() { ContextName = contextName, Namespace = contextNamespace }.TransformText();

            //            Project.AddContentToProject(contextFileName, context);
            //        }

            //        //call enable migrations
            //        //InvokeScript("Enable-Migrations");

            //    }

            //}

            OpenFile(Path.Combine(Project.GetProjectDir(), configurationFilePath));

            WriteLine(Strings.ModelEnableSuccessfull);
        }

        private void WriteResourceFile(string relativePath)
        {
            StringBuilder sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                using (var resWriter = new ResXResourceWriter(writer))
                {
                    //TODO: presunout tyhle magic stringy nekam globalne - pouziva je ModelMigrationsConfigurationUpdater a ModelMigrationsConfigurationBase
                    resWriter.AddResource("AppliedMigrations", "");
                    //TODO: pro builder db migraci budu vytvaret vlastni edmx pred aplikaci zmen -> prozatim si last aplied db migraci drzet nemusim
                    resWriter.AddResource("LastAppliedDbMigrationId", "");
                }
            }

            Project.AddContentToProject(relativePath, sb.ToString());
        }
    }
}
