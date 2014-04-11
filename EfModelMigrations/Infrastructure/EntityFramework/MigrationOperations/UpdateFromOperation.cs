using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    //TODO: az bude implementace tak upravit generatory - ExtendedCSharpMigrationCodeGenerator, Sqlgenerator atd...
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
