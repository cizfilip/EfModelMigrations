using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.PowerShellDispatcher;
using EnvDTE;
using System.Reflection;
using EfModelMigrations.Runtime.Infrastructure;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Properties;

namespace EfModelMigrations.Runtime.PowerShell
{
    internal abstract class PowerShellCommand
    {
        private readonly AppDomain domain;
        private readonly Dispatcher dispatcher;

        //private readonly string efDllPath;

        protected PowerShellCommand()
        {
            this.domain = AppDomain.CurrentDomain;
            this.dispatcher = (Dispatcher)domain.GetData("dispatcher");

            //this.efDllPath = (string)domain.GetData("efDllPath");
        }


        //TODO: Podpora rozdělení modelu/contextu/migrací do více projektů
        private Project project;
        public Project Project
        {
            get 
            {
                if (project == null)
                {
                    project = (Project)domain.GetData("project"); 
                }
                return project;
            }
        }

        //public Project StartUpProject
        //{
        //    get { return (Project)domain.GetData("startUpProject"); }
        //}

        //public Project ContextProject
        //{
        //    get { return (Project)domain.GetData("contextProject"); }
        //}

        protected AppDomain Domain
        {
            get { return domain; }
        }

       


        public void Execute()
        {
            //DebugCheck.NotNull(command);

            Init();

            try
            {
                //Build project before executing command
                BuildProject();
                ExecuteCore();
            }
            catch (Exception ex)
            {
                Throw(ex);
            }
        }



        protected abstract void ExecuteCore();

        //protected virtual NewAppDomainExecutor CreateExecutor()
        //{
        //    return new NewAppDomainExecutor(Project.GetAssemblyPath());
        //}

        protected virtual CommandFacade CreateFacade()
        {
            return new CommandFacade(Project)
                {
                    LogInfo = WriteLine,
                    LogWarning = WriteWarning,
                    LogVerbose = WriteVerbose
                };
        }

        public virtual void WriteLine(string message)
        {
            //DebugCheck.NotEmpty(message);

            dispatcher.WriteLine(message);
        }

        public virtual void WriteWarning(string message)
        {
            //DebugCheck.NotEmpty(message);

            dispatcher.WriteWarning(message);
        }

        public void WriteVerbose(string message)
        {
            //DebugCheck.NotEmpty(message);

            dispatcher.WriteVerbose(message);
        }

        public void OpenFile(string absolutePath)
        {
            dispatcher.OpenFile(absolutePath);
        }

        public void InvokeScript(string script)
        {
            dispatcher.InvokeScript(script);
        }

        private void Init()
        {
            domain.SetData("wasError", false);
            domain.SetData("error.Message", null);
            domain.SetData("error.TypeName", null);
            domain.SetData("error.StackTrace", null);
        }

        private void Throw(Exception ex)
        {
            //DebugCheck.NotNull(ex);

            domain.SetData("wasError", true);
            domain.SetData("error.Message", ex.Message);
            domain.SetData("error.TypeName", ex.GetType().FullName);
            domain.SetData("error.StackTrace", ex.ToString());
        }

        private void BuildProject()
        {
            Project.Build(() => new ModelMigrationsException(Resources.CannotBuildProject));
        }
        
    }
    
}
