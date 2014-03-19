using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure
{
    internal sealed class RunnerLogger : MarshalByRefObject
    {
        private CommandFacade facade;

        public RunnerLogger(CommandFacade facade)
        {
            this.facade = facade;
        }

        public void Info(string message)
        {
            if(facade.LogInfo != null)
            {
                facade.LogInfo(message);
            }
        }

        public void Warning(string message)
        {
            if (facade.LogWarning != null)
            {
                facade.LogWarning(message);
            }
        }

        public void Verbose(string message)
        {
            if (facade.LogVerbose != null)
            {
                facade.LogVerbose(message);
            }
        }
    }
}
