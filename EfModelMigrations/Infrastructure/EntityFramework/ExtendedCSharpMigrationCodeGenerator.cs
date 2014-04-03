using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Linq;

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
                    operation is MoveDataOperation)
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


        protected virtual void Generate(MoveDataOperation moveDataOperation, IndentedTextWriter writer)
        {
            writer.Write("this.");
            writer.Write("MoveData");
            writer.WriteLine("()");

            writer.Indent++;

            GenerateMoveDataFluentCall("FromTable", moveDataOperation.From, writer);
            writer.WriteLine();
            GenerateMoveDataFluentCall("ToTable", moveDataOperation.To, writer);
            writer.WriteLine(";");

            writer.Indent--;

            writer.WriteLine();
        }

        private void GenerateMoveDataFluentCall(string methodName, MoveDataModel model, IndentedTextWriter writer)
        {
            writer.Write(".");
            writer.Write(methodName);
            writer.Write("(");
            writer.Write(Quote(model.TableName));
            writer.Write(", new[] { ");

            foreach (var column in model.ColumnNames)
            {
                writer.Write(Quote(column));
                writer.Write(", ");
            }

            writer.Write("})");
        }

        protected virtual void Generate(AddIdentityOperation addIdentityOperation, IndentedTextWriter writer)
        {
            GenerateIdentityOperation("AddIdentity", addIdentityOperation, writer);
        }

        protected virtual void Generate(DropIdentityOperation dropIdentityOperation, IndentedTextWriter writer)
        {
            GenerateIdentityOperation("DropIdentity", dropIdentityOperation, writer);
        }

        private void GenerateIdentityOperation(string extensionMethodName, IdentityOperation operation, IndentedTextWriter writer)
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
