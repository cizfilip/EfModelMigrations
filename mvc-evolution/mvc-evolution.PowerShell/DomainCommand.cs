using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using mvc_evolution.PowerShell.Dispatcher;
using System.Reflection;
using System.IO;

namespace mvc_evolution.PowerShell
{
    internal abstract class DomainCommand
    {
        private readonly AppDomain domain;
        private readonly DomainDispatcher dispatcher;
        private readonly string efDllPath;

        protected DomainCommand()
        {
            this.domain = AppDomain.CurrentDomain;
            this.dispatcher = (DomainDispatcher)domain.GetData("dispatcher");

            this.efDllPath = (string)domain.GetData("efDllPath");
        }

        public virtual Project Project
        {
            get { return (Project)domain.GetData("project"); }
        }

        public Project StartUpProject
        {
            get { return (Project)domain.GetData("startUpProject"); }
        }

        public Project ContextProject
        {
            get { return (Project)domain.GetData("contextProject"); }
        }

        protected AppDomain Domain
        {
            get { return domain; }
        }

        protected Assembly EFAssembly
        {
            get
            {
                return LoadAssemblyFromFile(efDllPath);               
            }
        }

        

        public void Execute()
        {
            //DebugCheck.NotNull(command);

            Init();

            try
            {
                ExecuteCore();
            }
            catch (Exception ex)
            {
                Throw(ex);
            }
        }

        protected abstract void ExecuteCore();

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


        protected Assembly LoadAssemblyFromFile(string path)
        {
            //TODO: Rethrow with our exception if FileNotFound
            return Assembly.LoadFile(path);  
        }
    }
}
