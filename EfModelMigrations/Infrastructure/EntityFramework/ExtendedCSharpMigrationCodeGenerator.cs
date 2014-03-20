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
            return base.Generate(migrationId, NewOperations, sourceModel, targetModel, @namespace, className);
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
            GenerateMoveDataFluentCall("ToTable", moveDataOperation.From, writer);
            writer.WriteLine(";");
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
            writer.Write(", ");
            writer.Write(Quote(operation.PrincipalColumn));
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
