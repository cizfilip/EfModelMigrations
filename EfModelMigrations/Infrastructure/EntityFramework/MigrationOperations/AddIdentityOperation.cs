using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public class AddIdentityOperation : IdentityOperation
    {
        public AddIdentityOperation(object anonymousArguments = null)
            : base(anonymousArguments)
        {
        }

        public override MigrationOperation Inverse
        {
            get
            {
                var dropIdentityOperation
                    = new DropIdentityOperation
                    {
                        PrincipalColumn = PrincipalColumn,
                        PrincipalTable = PrincipalTable,
                    };

                dropIdentityOperation.DependentColumns.AddRange(this.DependentColumns);
                                
                return dropIdentityOperation;
            }
        }
    }
}
