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

        public ModelMigratorHelper MigratorHelper { get; set; }

        public HistoryTracker HistoryTracker { get; set; }

        public VsProjectBuilder ProjectBuilder { get; set; }

        private ModelMigrator migrator;
        public ModelMigrator Migrator
        {
            get
            {
                if(migrator == null)
                {
                    //TODO: vytvorit decorator migratoru ktery bude vypisovat uzivateli pomoci RunnerLogger - Ne tohle fakt decorator...
                    migrator = new LoggingModelMigrator(
                        HistoryTracker,
                        MigratorHelper,
                        edmx => new VsClassModelProvider(ModelProject, Configuration, edmx),
                        new VsModelChangesExecutor(HistoryTracker, ModelProject, Configuration),
                        Configuration,
                        ProjectBuilder,
                        new DbMigrationWriter(ModelProject),
                        ModelProject.GetProjectDir()
                        )
                        {
                            Logger = Log
                        };
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
