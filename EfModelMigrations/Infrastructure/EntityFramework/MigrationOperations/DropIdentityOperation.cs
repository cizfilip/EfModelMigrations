using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public class DropIdentityOperation : IdentityOperation
    {
        public DropIdentityOperation(object anonymousArguments = null)
            : base(anonymousArguments)
        {
        }

        public override MigrationOperation Inverse
        {
            get
            {
                var addIdentityOperation 
                    = new AddIdentityOperation
                        {
                            PrincipalColumn = PrincipalColumn,
                            PrincipalTable = PrincipalTable,
                        };
                foreach (var dependentColumn in DependentColumns)
                {
                    addIdentityOperation.DependentColumns.Add(dependentColumn);
                }

                return addIdentityOperation;
            }
        }

    }
}
