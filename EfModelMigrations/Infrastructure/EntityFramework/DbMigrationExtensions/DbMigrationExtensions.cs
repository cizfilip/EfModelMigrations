using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Builders;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;
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
                Func<ColumnBuilder, ColumnModel> principalColumnAction)
        {
            var operation = new AddIdentityOperation
            {
                PrincipalTable = principalTable,
                PrincipalColumn = principalColumnAction(new ColumnBuilder())
            };

            ((IDbMigration)migration).AddOperation(operation);

            return new IdentityOperationWrapper(operation);
        }

        public static IdentityOperationWrapper DropIdentity(
                this DbMigration migration,
                string principalTable,
                Func<ColumnBuilder, ColumnModel> principalColumnAction)
        {
            var operation = new DropIdentityOperation
            {
                PrincipalTable = principalTable,
                PrincipalColumn = principalColumnAction(new ColumnBuilder())
            };

            ((IDbMigration)migration).AddOperation(operation);

            return new IdentityOperationWrapper(operation);
        }

        public class IdentityOperationWrapper : IFluentInterface
        {
            private IdentityOperation operation;

            public IdentityOperationWrapper(IdentityOperation operation)
            {
                this.operation = operation;
            }

            public IdentityOperationWrapper WithDependentColumn(
                string table,
                string foreignKeyColumn)
            {
                operation.DependentColumns.Add(new DependentColumn
                {
                    DependentTable = table,
                    ForeignKeyColumn = foreignKeyColumn
                });

                return this;
            }
        }


        public static InsertFromOperationWrapper InsertFrom(this DbMigration migration)
        {
            var operation = new InsertFromOperation();
            ((IDbMigration)migration).AddOperation(operation);
            return new InsertFromOperationWrapper(operation);
        }

        public class InsertFromOperationWrapper : IFluentInterface
        {
            private InsertFromOperation operation;

            public InsertFromOperationWrapper(InsertFromOperation operation)
            {
                this.operation = operation;
            }

            public InsertFromOperationWrapper FromTable(
                string table,
                string[] columns)
            {
                operation.From = new InsertDataModel(table, columns);
                return this;
            }

            public InsertFromOperationWrapper ToTable(
                string table,
                string[] columns)
            {
                operation.To = new InsertDataModel(table, columns);
                return this;
            }
        }
    }
}
