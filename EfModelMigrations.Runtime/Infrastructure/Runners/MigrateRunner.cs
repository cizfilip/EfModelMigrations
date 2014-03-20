using EfModelMigrations.Runtime.Infrastructure.Migrations;
using EfModelMigrations.Runtime.Infrastructure.Runners.Migrators;
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

        public EdmxModelProvider EdmxProvider { get; set; }

        private ModelMigrator migrator;
        public ModelMigrator Migrator
        {
            get
            {
                if(migrator == null)
                {
                    //migrator = new ModelMigrator();
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
