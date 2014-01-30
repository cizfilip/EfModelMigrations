using EfModelMigrations.Runtime.Infrastructure.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private AppDomain newDomain;

        private string projectAssemblyPath;

        public NewAppDomainExecutor(string projectAssemblyPath)
        {
            this.projectAssemblyPath = projectAssemblyPath;

            var baseDomainSetup = AppDomain.CurrentDomain.SetupInformation;

            var setup = new AppDomainSetup
            {
                ApplicationBase = baseDomainSetup.ApplicationBase,
                PrivateBinPath = baseDomainSetup.PrivateBinPath,
                ShadowCopyFiles = "true"
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
