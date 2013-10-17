using EnvDTE;
using EfModelMigrations.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Runtime.Infrastructure.Migrations;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal class UpdateConfigurationRunner : BaseRunner
    {
        public Project MigrationProject { get; set; }
        public bool IsRevert { get; set; }
        public string AppliedModelMigrationId { get; set; }
        public string AppliedDbMigrationId { get; set; }

        public override void Run()
        {
            string configurationResourcePath = Path.Combine(MigrationProject.GetProjectDir(), Configuration.ModelMigrationsDirectory, Configuration.GetType().Name + ".resx");
            var updater = new ModelMigrationsConfigurationUpdater(configurationResourcePath);

            if (!IsRevert)
            {
                updater.AddAppliedMigration(AppliedModelMigrationId, AppliedDbMigrationId);
            }
            else
            {
                updater.RemoveLastAppliedMigration(AppliedDbMigrationId);
            }
        }
    }
}
