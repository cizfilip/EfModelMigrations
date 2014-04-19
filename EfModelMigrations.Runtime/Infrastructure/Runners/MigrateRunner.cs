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

        private ModelMigratorBase migrator;
        public ModelMigratorBase Migrator
        {
            get
            {
                if(migrator == null)
                {
                    var migratorForDecoration = new ModelMigrator(
                        HistoryTracker,
                        MigratorHelper,
                        modelMetadata => new VsClassModelProvider(ModelProject, Configuration, modelMetadata),
                        new VsModelChangesExecutor(HistoryTracker, ModelProject, Configuration),
                        Configuration,
                        ProjectBuilder,
                        new DbMigrationWriter(ModelProject),
                        ModelProject.GetProjectDir()
                        );

                    migrator = DecorateMigrator(migratorForDecoration);
                }
                return migrator;
            }
        }



        public override void Run()
        {
            Migrator.Migrate(TargetMigration, Force);
        }


        public ModelMigratorBase DecorateMigrator(ModelMigrator migrator)
        {
            return new LoggingModelMigrator(migrator, Log);
        }
    }
}
