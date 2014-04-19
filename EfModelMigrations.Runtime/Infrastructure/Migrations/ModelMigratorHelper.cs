using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Runtime.Infrastructure.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    /// <summary>
    /// Helper class used by ModelMigrator for operations during migrating which must run in new AppDomain
    /// </summary>
    internal sealed class ModelMigratorHelper : MarshalByRefObject
    {
        private Func<NewAppDomainExecutor> executorFactory;

        public ModelMigratorHelper(Func<NewAppDomainExecutor> executorFactory)
        {
            this.executorFactory = executorFactory;
        }

        public void UpdateDatabase(string targetDbMigration)
        {
            using (var executor = executorFactory())
            {
                executor.ExecuteRunner(new UpdateDatabaseRunner()
                {
                    TargetDbMigration = targetDbMigration
                });
            }
        }

        public string GetEdmxModel()
        {
            using (var executor = executorFactory())
            {
                return executor.ExecuteRunner<string>(new GetEdmxRunner());
            }
        }
    }
}
