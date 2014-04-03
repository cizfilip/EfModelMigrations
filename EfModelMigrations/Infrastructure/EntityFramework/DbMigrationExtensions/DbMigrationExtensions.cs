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


        public static MoveDataOperationWrapper MoveData(this DbMigration migration)
        {
            var operation = new MoveDataOperation();
            ((IDbMigration)migration).AddOperation(operation);
            return new MoveDataOperationWrapper(operation);
        }

        public class MoveDataOperationWrapper : IFluentInterface
        {
            private MoveDataOperation operation;

            public MoveDataOperationWrapper(MoveDataOperation operation)
            {
                this.operation = operation;
            }

            public MoveDataOperationWrapper FromTable(
                string table,
                string[] columns)
            {
                operation.From = new MoveDataModel(table, columns);
                return this;
            }

            public MoveDataOperationWrapper ToTable(
                string table,
                string[] columns)
            {
                operation.To = new MoveDataModel(table, columns);
                return this;
            }
        }
    }
}
