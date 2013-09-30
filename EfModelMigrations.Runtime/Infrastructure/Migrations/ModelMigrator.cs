using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    internal class ModelMigrator
    {
        private Func<NewAppDomainExecutor> executorFactory;

        public ModelMigrator(Func<NewAppDomainExecutor> executorFactory)
        {
            this.executorFactory = executorFactory;
        }

        public void ApplyMigrations(IEnumerable<string> migrationIds)
        {
            foreach (var migrationId in migrationIds)
            {
                Migrate(migrationId, isRevert: false);
            }
        }
        public void RevertMigrations(IEnumerable<string> migrationIds)
        {
            foreach (var migrationId in migrationIds)
            {
                Migrate(migrationId, isRevert: true);
            }
        }



        private void Migrate(string migratonId, bool isRevert)
        {
            //apply model changes
            using (var executor = executorFactory())
            {
                //executor.ExecuteRunner(new )
            }
        }
    }
}
