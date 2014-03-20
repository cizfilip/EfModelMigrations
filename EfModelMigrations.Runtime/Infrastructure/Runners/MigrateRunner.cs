using EfModelMigrations.Runtime.Infrastructure.Migrations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Runtime.Extensions;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners
{
    [Serializable]
    internal class MigrateRunner : BaseRunner
    {
        public string TargetMigration { get; set; }

        public bool Force { get; set; }

        public Project ModelProject { get; set; }

        public EdmxModelProvider EdmxProvider { get; set; }
        public DatabaseUpdater DatabaseUpdater { get; set; }

        public HistoryTracker HistoryTracker { get; set; }

        public VsProjectBuilder ProjectBuilder { get; set; }

        private ModelMigrator migrator;
        public ModelMigrator Migrator
        {
            get
            {
                if(migrator == null)
                {
                    //TODO: vytvorit decorator migratoru ktery bude vypisovat uzivateli pomoci RunnerLogger
                    migrator = new ModelMigrator(
                        HistoryTracker,
                        EdmxProvider,
                        new VsClassModelProvider(ModelProject, Configuration),
                        new VsModelChangesExecutor(HistoryTracker, ModelProject, Configuration),
                        Configuration,
                        ProjectBuilder,
                        DatabaseUpdater,
                        new DbMigrationWriter(ModelProject),
                        ModelProject.GetProjectDir()
                        );
                }
                return migrator;
            }
        }



        public override void Run()
        {
            Migrator.Migrate(TargetMigration, Force);
        }

    }
}
