using EfModelMigrations.Runtime.Infrastructure.Runners;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure
{
    /// <summary>
    /// Helper class that execute Run method from BaseRunner subclasses in new appdomain.
    /// Everything which need access to types in project dll must be executed in new 
    /// appdomain using this class, because dll cannot be unloaded only appdomain can.
    /// </summary>
    internal class NewAppDomainExecutor : IDisposable
    {
        internal AppDomain newDomain;

        private string projectAssemblyPath;
        
        private RunnerLogger logger;

        public NewAppDomainExecutor(string workingDirectory, string configurationFilePath, string projectAssemblyPath, RunnerLogger logger)
        {
            this.logger = logger;
            this.projectAssemblyPath = projectAssemblyPath;

            var baseDomainSetup = AppDomain.CurrentDomain.SetupInformation;

            var setup = new AppDomainSetup
            {
                ApplicationBase = baseDomainSetup.ApplicationBase,
                PrivateBinPath = baseDomainSetup.PrivateBinPath,
                ConfigurationFile = configurationFilePath,
                ShadowCopyFiles = "true",
                ShadowCopyDirectories = baseDomainSetup.ApplicationBase + ";" + baseDomainSetup.PrivateBinPath + ";" + workingDirectory
            };

            
            var friendlyName = "EfModelMigrationsNewDomain" + Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            newDomain = AppDomain.CreateDomain(friendlyName, null, setup);
        }

        ~NewAppDomainExecutor()
        {
            Dispose(false);
        }

        public T ExecuteRunner<T>(BaseRunner runner)
        {
            ExecuteRunner(runner);

            return (T)newDomain.GetData(BaseRunner.ResultKey);
        }

        public void ExecuteRunner(BaseRunner runner)
        {
            ConfigureRunner(runner);

            newDomain.DoCallBack(runner.Run);
        }



        private void ConfigureRunner(BaseRunner runner)
        {
            runner.ProjectAssemblyPath = projectAssemblyPath;
            runner.Log = logger;
        }


        #region IDisposable implementation

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Unloads appdomain
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && newDomain != null)
            {
                AppDomain.Unload(newDomain);
                newDomain = null;
            }
        }

        #endregion
    }
}
