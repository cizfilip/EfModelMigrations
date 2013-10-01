using EfModelMigrations.Runtime.Infrastructure.Runners.Migrators;
using EnvDTE;
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
        private Project modelProject;

        public ModelMigrator(Project modelProject, Func<NewAppDomainExecutor> executorFactory)
        {
            this.executorFactory = executorFactory;
            this.modelProject = modelProject;
        }

        public void Migrate(IEnumerable<string> migrationIds, bool isRevert)
        {
            foreach (var migrationId in migrationIds)
            {
                Migrate(migrationId, isRevert);
            }
        }
        

        private void Migrate(string migratonId, bool isRevert)
        {
            //apply model changes
            using (var executor = executorFactory())
            {
                executor.ExecuteRunner(new ApplyModelChangesRunner()
                {
                    ModelProject = modelProject,
                    ModelMigrationId = migratonId,
                    IsRevert = isRevert
                });
            }
        }
    }
}
