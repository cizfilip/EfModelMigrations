﻿using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Runtime.Infrastructure.Runners;
using EfModelMigrations.Runtime.Extensions;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Runtime.Infrastructure.Runners.TypeFinders;
using EfModelMigrations.Runtime.Infrastructure.Migrations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;

namespace EfModelMigrations.Runtime.Infrastructure
{
    internal sealed class CommandFacade : IDisposable
    {
        private NewAppDomainExecutor executor;
        private Project modelProject;
        private string configurationFilePath;
        private string applicationBase;
        private string projectAssemblyPath;

        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarning { get; set; }
        public Action<string> LogVerbose { get; set; }


        public CommandFacade(Project modelProject)
        {
            this.modelProject = modelProject;

            //TODO: configfile musi byt ze startup projectu
            if (modelProject.IsWebProject())
            {
                this.configurationFilePath = modelProject.GetFileName("Web.config");
            }
            else
            {
                this.configurationFilePath = modelProject.GetFileName("App.config");
            }
            this.applicationBase = modelProject.GetTargetDir();
            this.projectAssemblyPath = modelProject.GetAssemblyPath();

            this.executor = CreateExecutor();
        }


        public GeneratedModelMigration GenerateMigration(string commandFullName, bool isRescaffold, string migrationName, object[] parameters)
        {
            return executor.ExecuteRunner<GeneratedModelMigration>(new GenerateMigrationFromCommandRunner() 
                        { 
                            ModelProject = this.modelProject,
                            CommandFullName = commandFullName,
                            MigrationName = migrationName,
                            IsRescaffold = isRescaffold,
                            Parameters = parameters
                        });
        }


        public void Migrate(string targetModelMigration, bool force)
        {
            executor.ExecuteRunner(new MigrateRunner()
            {
                TargetMigration = targetModelMigration,
                Force = force,
                MigratorHelper = new ModelMigratorHelper(CreateExecutor),
                HistoryTracker = new HistoryTracker(modelProject),
                ModelProject = modelProject,
                ProjectBuilder = new VsProjectBuilder(modelProject)
            });
        }


        public string FindModelMigrationsConfiguration()
        {
            return executor.ExecuteRunner<string>(new FindModelMigrationsConfigurationRunner());
        }

        private NewAppDomainExecutor CreateExecutor()
        {
            return new NewAppDomainExecutor( 
                applicationBase,
                configurationFilePath, 
                projectAssemblyPath,
                new RunnerLogger(this));
        }

        public void Dispose()
        {
            if(executor != null)
            {
                executor.Dispose();
            }
        }
    }
}
