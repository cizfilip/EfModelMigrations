using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Linq;
using System;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public class ExtendedCSharpMigrationCodeGenerator : CSharpMigrationCodeGenerator
    {
        public IEnumerable<MigrationOperation> NewOperations { get; set; }
        
        public override ScaffoldedMigration Generate(string migrationId, IEnumerable<MigrationOperation> operations, string sourceModel, string targetModel, string @namespace, string className)
        {
            //Hax used because of dynamic dispatch from base generator not work in derived generator, so we pretending with PlaceholderOperation that all new operations is SqlOperation
            var newOperations = NewOperations.ToList();
            for (int i = 0; i < newOperations.Count; i++)
            {
                var operation = newOperations[i];
                if(operation is AddIdentityOperation ||
                    operation is DropIdentityOperation ||
                    operation is InsertFromOperation || 
                    operation is UpdateFromOperation)
                {
                    newOperations[i] = new PlaceholderOperation(operation);
                }
            }
            return base.Generate(migrationId, newOperations, sourceModel, targetModel, @namespace, className);
        }

        protected override void Generate(SqlOperation sqlOperation, IndentedTextWriter writer)
        {
            if(sqlOperation is PlaceholderOperation)
            {
                dynamic opToGenerate = ((PlaceholderOperation)sqlOperation).UnderlayingOperation;
                Generate(opToGenerate, writer);
            }
            else
            {
                base.Generate(sqlOperation, writer);
            }
        }

        protected override IEnumerable<string> GetNamespaces(IEnumerable<MigrationOperation> operations)
        {
            var addedNamespaces = new List<string>();

            addedNamespaces.Add("EfModelMigrations.Infrastructure.EntityFramework.DbMigrationExtensions");

            //TODO: pridat namespacy s extension metodama pro nove databazove migracni operace
            return base.GetNamespaces(operations).Concat(addedNamespaces);
        }


        protected virtual void Generate(InsertFromOperation insertFromOperation, IndentedTextWriter writer)
        {
            GenerateInsertOrUpdateFrom<InsertFromOperation, InserFromDataModel>("InsertFrom", insertFromOperation, GenerateInsertFromFluentCall, writer);
        }

        private void GenerateInsertFromFluentCall(string methodName, InserFromDataModel model, IndentedTextWriter writer)
        {
            GenerateInsertOrUpdateFromFluentCall(methodName, model.TableName, model.ColumnNames, writer);
            writer.Write(")");
        }        

        protected virtual void Generate(UpdateFromOperation updateFromOperation, IndentedTextWriter writer)
        {
            GenerateInsertOrUpdateFrom<UpdateFromOperation, UpdateFromDataModel>("UpdateFrom", updateFromOperation, GenerateUpdateFromFluentCall, writer);
        }

        private void GenerateUpdateFromFluentCall(string methodName, UpdateFromDataModel model, IndentedTextWriter writer)
        {
            GenerateInsertOrUpdateFromFluentCall(methodName, model.TableName, model.ColumnNames, writer);

            writer.Write(", new[] { ");
            foreach (var column in model.JoinColumns)
            {
                writer.Write(Quote(column));
                writer.Write(", ");
            }

            writer.Write("})");
        }

        private void GenerateInsertOrUpdateFromFluentCall(string methodName, string tableName, string[] columnNames, IndentedTextWriter writer) 
        {
            writer.Write(".");
            writer.Write(methodName);
            writer.Write("(");
            writer.Write(Quote(tableName));
            writer.Write(", new[] { ");

            foreach (var column in columnNames)
            {
                writer.Write(Quote(column));
                writer.Write(", ");
            }

            writer.Write("}");
        }

        protected virtual void GenerateInsertOrUpdateFrom<T, TData>(string methodName, T operation, Action<string, TData, IndentedTextWriter> generateFluentCallAction, IndentedTextWriter writer)
            where TData : class
            where T : MoveDataOperation<TData>
        {
            writer.Write("this.");
            writer.Write(methodName);
            writer.WriteLine("()");

            writer.Indent++;

            generateFluentCallAction("FromTable", operation.From, writer);
            writer.WriteLine();
            generateFluentCallAction("ToTable", operation.To, writer);
            writer.WriteLine(";");

            writer.Indent--;

            writer.WriteLine();
        }

        protected virtual void Generate(AddIdentityOperation addIdentityOperation, IndentedTextWriter writer)
        {
            GenerateIdentityOperation("AddIdentity", addIdentityOperation, writer);
        }

        protected virtual void Generate(DropIdentityOperation dropIdentityOperation, IndentedTextWriter writer)
        {
            GenerateIdentityOperation("DropIdentity", dropIdentityOperation, writer);
        }

        protected virtual void GenerateIdentityOperation(string extensionMethodName, IdentityOperation operation, IndentedTextWriter writer)
        {
            writer.Write("this.");
            writer.Write(extensionMethodName);
            writer.Write("(");
            writer.Write(Quote(operation.PrincipalTable));
            writer.Write(", c =>");
            Generate(operation.PrincipalColumn, writer);
            writer.Write(")");

            if (operation.DependentColumns.Count > 0)
            {
                writer.Indent++;

                foreach (var dependentColumn in operation.DependentColumns)
                {
                    writer.WriteLine();
                    writer.Write(".WithDependentColumn(");
                    writer.Write(Quote(dependentColumn.DependentTable));
                    writer.Write(", ");
                    writer.Write(Quote(dependentColumn.ForeignKeyColumn));
                    writer.Write(")");
                }

                writer.Indent--;
            }

            writer.WriteLine(";");
        }

    }
}
