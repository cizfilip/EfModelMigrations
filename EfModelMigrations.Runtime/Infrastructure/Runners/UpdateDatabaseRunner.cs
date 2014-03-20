using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners
{
    [Serializable]
    internal class UpdateDatabaseRunner : BaseRunner
    {
        public string TargetDbMigration { get; set; }

        public override void Run()
        {
            DbMigrator dbMigrator = new DbMigrator(DbConfiguration);

            dbMigrator.Update(TargetDbMigration);
        }
    }
}
