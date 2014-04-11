using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public class UpdateFromOperation : MigrationOperation
    {
        

        public UpdateFromOperation(object anonymousArguments = null)
            : base(anonymousArguments)
        {
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
