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
        public static IIdentityOperationWrapper AddIdentity(
                this DbMigration migration,
                string principalTable,
                string principalColumnName,
                Func<ColumnBuilder, ColumnModel> principalColumnAction)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(principalTable, "principalTable");
            Check.NotNull(principalColumnAction, "principalColumnAction");

            return CreateIdentityOperation(migration, new AddIdentityOperation(), principalTable, principalColumnName, principalColumnAction);
        }

        public static IIdentityOperationWrapper DropIdentity(
                this DbMigration migration,
                string principalTable,
                string principalColumnName,
                Func<ColumnBuilder, ColumnModel> principalColumnAction)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(principalTable, "principalTable");
            Check.NotNull(principalColumnAction, "principalColumnAction");

            return CreateIdentityOperation(migration, new DropIdentityOperation(), principalTable, principalColumnName, principalColumnAction);
        }

        private static IdentityOperationWrapper CreateIdentityOperation(
                DbMigration migration,
                IdentityOperation operation,
                string principalTable,
                string principalColumnName,
                Func<ColumnBuilder, ColumnModel> principalColumnAction)
        {
            operation.PrincipalTable = principalTable;
            operation.PrincipalColumn = principalColumnAction(new ColumnBuilder());
            operation.PrincipalColumn.Name = principalColumnName;

            ((IDbMigration)migration).AddOperation(operation);

            return new IdentityOperationWrapper(operation);
        }

        public static IInsertFromOperationWrapper InsertFrom(this DbMigration migration)
        {
            Check.NotNull(migration, "migration");

            var operation = new InsertFromOperation();
            ((IDbMigration)migration).AddOperation(operation);
            return new InsertFromOperationWrapper(operation);
        }

        public static IUpdateFromOperationWrapper UpdateFrom(this DbMigration migration)
        {
            Check.NotNull(migration, "migration");

            var operation = new UpdateFromOperation();
            ((IDbMigration)migration).AddOperation(operation);
            return new UpdateFromOperationWrapper(operation);
        }
    }

    //FluentApiWrappers
    public interface IIdentityOperationWrapper : IFluentInterface
    {
        IIdentityOperationWrapper WithDependentColumn(string table, string foreignKeyColumn);
    }

    public class IdentityOperationWrapper : IIdentityOperationWrapper
    {
        private IdentityOperation operation;

        public IdentityOperationWrapper(IdentityOperation operation)
        {
            Check.NotNull(operation, "operation");

            this.operation = operation;
        }

        public IIdentityOperationWrapper WithDependentColumn(
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

    public class MoveDataOperationWrapper<T> where T : class
    {
        protected MoveDataOperation<T> operation;

        public MoveDataOperationWrapper(MoveDataOperation<T> operation)
        {
            Check.NotNull(operation, "operation");

            this.operation = operation;
        }
    }

    public interface IInsertFromOperationWrapper : IFluentInterface
    {
        IInsertFromOperationWrapper FromTable(string table, string[] columns);
        IInsertFromOperationWrapper ToTable(string table, string[] columns);
    }

    public class InsertFromOperationWrapper : MoveDataOperationWrapper<InserFromDataModel>, IInsertFromOperationWrapper
    {
        public InsertFromOperationWrapper(InsertFromOperation operation)
            : base(operation)
        {
        }

        public IInsertFromOperationWrapper FromTable(
            string table,
            string[] columns)
        {
            Check.NotEmpty(table, "table");
            Check.NotNullOrEmpty(columns, "columns");

            operation.From = new InserFromDataModel(table, columns);
            return this;
        }

        public IInsertFromOperationWrapper ToTable(
            string table,
            string[] columns)
        {
            Check.NotEmpty(table, "table");
            Check.NotNullOrEmpty(columns, "columns");

            operation.To = new InserFromDataModel(table, columns);
            return this;
        }
    }

    public interface IUpdateFromOperationWrapper : IFluentInterface
    {
        IUpdateFromOperationWrapper FromTable(string table, string[] columns, string[] joinColumns);
        IUpdateFromOperationWrapper ToTable(string table, string[] columns, string[] joinColumns);
    }

    public class UpdateFromOperationWrapper : MoveDataOperationWrapper<UpdateFromDataModel>, IUpdateFromOperationWrapper
    {
        public UpdateFromOperationWrapper(UpdateFromOperation operation)
            :base(operation)
        {
        }

        public IUpdateFromOperationWrapper FromTable(
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

        public IUpdateFromOperationWrapper ToTable(
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
