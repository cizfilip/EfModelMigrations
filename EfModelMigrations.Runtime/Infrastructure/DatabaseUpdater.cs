using EfModelMigrations.Runtime.Infrastructure.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure
{
    internal class DatabaseUpdater : MarshalByRefObject
    {
        private Func<NewAppDomainExecutor> executorFactory;

        public DatabaseUpdater(Func<NewAppDomainExecutor> executorFactory)
        {
            this.executorFactory = executorFactory;
        }

        public void Update(string targetDbMigration)
        {
            using (var executor = executorFactory())
            {
                executor.ExecuteRunner(new UpdateDatabaseRunner()
                    {
                        TargetDbMigration = targetDbMigration
                    });
            }
        }
    }
}
