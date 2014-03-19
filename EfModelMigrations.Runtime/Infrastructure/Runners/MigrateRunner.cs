using EfModelMigrations.Runtime.Infrastructure.Runners.Migrators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners
{
    [Serializable]
    internal class MigrateRunner : MigratorBaseRunner
    {
        public string TargetMigration { get; set; }

        public bool Force { get; set; }


        public override void Run()
        {
            //Migrator.Migrate(TargetMigration, Force);
        }

    }
}
