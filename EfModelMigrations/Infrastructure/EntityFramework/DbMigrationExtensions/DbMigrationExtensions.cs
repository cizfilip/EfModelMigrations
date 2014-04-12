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
            Check.NotNull(migration, "migration");
            Check.NotEmpty(principalTable, "principalTable");
            Check.NotNull(principalColumnAction, "principalColumnAction");

            return CreateIdentityOperation(migration, new AddIdentityOperation(), principalTable, principalColumnAction);
        }

        public static IdentityOperationWrapper DropIdentity(
                this DbMigration migration,
                string principalTable,
                Func<ColumnBuilder, ColumnModel> principalColumnAction)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(principalTable, "principalTable");
            Check.NotNull(principalColumnAction, "principalColumnAction");

            return CreateIdentityOperation(migration, new DropIdentityOperation(), principalTable, principalColumnAction);
        }

        private static IdentityOperationWrapper CreateIdentityOperation(
                DbMigration migration,
                IdentityOperation operation,
                string principalTable,
                Func<ColumnBuilder, ColumnModel> principalColumnAction)
        {
            operation.PrincipalTable = principalTable;
            operation.PrincipalColumn = principalColumnAction(new ColumnBuilder());

            ((IDbMigration)migration).AddOperation(operation);

            return new IdentityOperationWrapper(operation);
        }

        public static InsertFromOperationWrapper InsertFrom(this DbMigration migration)
        {
            Check.NotNull(migration, "migration");

            var operation = new InsertFromOperation();
            ((IDbMigration)migration).AddOperation(operation);
            return new InsertFromOperationWrapper(operation);
        }

        public static UpdateFromOperationWrapper UpdateFrom(this DbMigration migration)
        {
            Check.NotNull(migration, "migration");

            var operation = new UpdateFromOperation();
            ((IDbMigration)migration).AddOperation(operation);
            return new UpdateFromOperationWrapper(operation);
        }
    }

    //FluentApiWrappers
    public class IdentityOperationWrapper : IFluentInterface
    {
        private IdentityOperation operation;

        public IdentityOperationWrapper(IdentityOperation operation)
        {
            Check.NotNull(operation, "operation");

            this.operation = operation;
        }

        public IdentityOperationWrapper WithDependentColumn(
            string table,
            string foreignKeyColumn)
        {
            Check.NotEmpty(table, "table");
            Check.NotEmpty(foreignKeyColumn, "foreignKeyColumn");

            operation.DependentColumns.Add(new DependentColumn
            {
                DependentTable = table,
                ForeignKeyColumn = foreignKeyColumn
            });

            return this;
        }
    }

    public class MoveDataOperationWrapper<T> : IFluentInterface where T : class
    {
        protected MoveDataOperation<T> operation;

        public MoveDataOperationWrapper(MoveDataOperation<T> operation)
        {
            Check.NotNull(operation, "operation");

            this.operation = operation;
        }
    }

    public class InsertFromOperationWrapper : MoveDataOperationWrapper<InserFromDataModel>
    {
        public InsertFromOperationWrapper(InsertFromOperation operation)
            : base(operation)
        {
        }

        public InsertFromOperationWrapper FromTable(
            string table,
            string[] columns)
        {
            Check.NotEmpty(table, "table");
            Check.NotNullOrEmpty(columns, "columns");

            operation.From = new InserFromDataModel(table, columns);
            return this;
        }

        public InsertFromOperationWrapper ToTable(
            string table,
            string[] columns)
        {
            Check.NotEmpty(table, "table");
            Check.NotNullOrEmpty(columns, "columns");

            operation.To = new InserFromDataModel(table, columns);
            return this;
        }
    }

    public class UpdateFromOperationWrapper : MoveDataOperationWrapper<UpdateFromDataModel>
    {
        public UpdateFromOperationWrapper(UpdateFromOperation operation)
            :base(operation)
        {
        }

        public UpdateFromOperationWrapper FromTable(
            string table,
            string[] columns,
            string[] joinColumns)
        {
            Check.NotEmpty(table, "table");
            Check.NotNullOrEmpty(columns, "columns");
            Check.NotNullOrEmpty(joinColumns, "joinColumns");

            operation.From = new UpdateFromDataModel(table, columns, joinColumns);
            return this;
        }

        public UpdateFromOperationWrapper ToTable(
            string table,
            string[] columns,
            string[] joinColumns)
        {
            Check.NotEmpty(table, "table");
            Check.NotNullOrEmpty(columns, "columns");
            Check.NotNullOrEmpty(joinColumns, "joinColumns");

            operation.To = new UpdateFromDataModel(table, columns, joinColumns);
            return this;
        }
    }
}
