using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.DbMigrationExtensions
{
    public static class DbMigrationExtensions
    {
        
        public static IdentityOperationWrapper AddIdentity(
                this DbMigration migration,
                string principalTable,
                string principalColumn)
        {
            var operation = new AddIdentityOperation
            {
                PrincipalTable = principalTable,
                PrincipalColumn = principalColumn,
            };

            ((IDbMigration)migration).AddOperation(operation);

            return new IdentityOperationWrapper(operation);
        }

        public static IdentityOperationWrapper DropIdentity(
                this DbMigration migration,
                string principalTable,
                string principalColumn)
        {
            var operation = new DropIdentityOperation
            {
                PrincipalTable = principalTable,
                PrincipalColumn = principalColumn,
            };

            ((IDbMigration)migration).AddOperation(operation);

            return new IdentityOperationWrapper(operation);
        }

        public class IdentityOperationWrapper
        {
            private IdentityOperation _operation;

            public IdentityOperationWrapper(IdentityOperation operation)
            {
                _operation = operation;
            }

            public IdentityOperationWrapper WithDependentColumn(
                string table,
                string foreignKeyColumn)
            {
                _operation.DependentColumns.Add(new DependentColumn
                {
                    DependentTable = table,
                    ForeignKeyColumn = foreignKeyColumn
                });

                return this;
            }
        }

    }
}
