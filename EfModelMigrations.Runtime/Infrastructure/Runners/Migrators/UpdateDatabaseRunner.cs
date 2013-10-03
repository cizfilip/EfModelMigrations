using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal class UpdateDatabaseRunner : MigratorBaseRunner
    {

        public string TargetDbMigration { get; set; }

        public override void Run()
        {
            DbMigrator dbMigrator = new DbMigrator(DbConfiguration);

            dbMigrator.Update(TargetDbMigration);
        }
    }
}
