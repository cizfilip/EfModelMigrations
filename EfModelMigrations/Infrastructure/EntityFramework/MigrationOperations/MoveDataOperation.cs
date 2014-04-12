using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public abstract class MoveDataOperation<T> : MigrationOperationWithInverse where T : class
    {
        public T From { get; set; }
        public T To { get; set; }

        public MoveDataOperation(MigrationOperation inverse, object anonymousArguments = null)
            : base(inverse, anonymousArguments)
        {
        }

        public MoveDataOperation(object anonymousArguments = null)
            : base(null, anonymousArguments)
        {
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
