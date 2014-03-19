using EfModelMigrations.Infrastructure.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure
{
    internal class CommandFacade : IDisposable
    {
        private NewAppDomainExecutor executor;


        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarning { get; set; }
        public Action<string> LogVerbose { get; set; }


        public CommandFacade(string projectAssemblyPath)
        {
            executor = new NewAppDomainExecutor(projectAssemblyPath);


        }


        public GeneratedModelMigration GenerateMigration()
        {
            throw new NotImplementedException();
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
