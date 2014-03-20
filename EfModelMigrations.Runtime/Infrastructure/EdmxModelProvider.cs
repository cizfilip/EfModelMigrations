using EfModelMigrations.Runtime.Infrastructure.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure
{
    internal sealed class EdmxModelProvider : MarshalByRefObject
    {
        private NewAppDomainExecutor executor;

        public EdmxModelProvider(NewAppDomainExecutor executor)
        {
            this.executor = executor;
        }

        public string GetEdmxModel()
        {
            return executor.ExecuteRunner<string>(new GetEdmxRunner());
        }
    }
}
