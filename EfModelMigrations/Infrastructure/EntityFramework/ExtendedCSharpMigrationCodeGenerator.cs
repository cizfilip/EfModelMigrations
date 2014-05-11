using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using EfModelMigrations.Extensions;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Linq;
using System;
using System.IO;
using System.Globalization;

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
            writer.Write(", ");
            writer.Write(Quote(operation.PrincipalColumn.Name));
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

        //Override for CreateTable - because Ef implementations merge fkconstraints and indexes which means that
        //model migration which does CreateClass and then AddAssoc generate db migrations which does not compile!
        protected override void Generate(CreateTableOperation createTableOperation, IndentedTextWriter writer)
        {
            Check.NotNull(createTableOperation, "createTableOperation");
            Check.NotNull(writer, "writer");

            writer.WriteLine("CreateTable(");
            writer.Indent++;
            writer.Write(Quote(createTableOperation.Name));
            writer.WriteLine(",");
            writer.WriteLine("c => new");
            writer.Indent++;
            writer.WriteLine("{");
            writer.Indent++;

            createTableOperation.Columns.Each(
                c =>
                {
                    var scrubbedName = ScrubName(c.Name);

                    writer.Write(scrubbedName);
                    writer.Write(" =");
                    Generate(c, writer, !string.Equals(c.Name, scrubbedName, StringComparison.Ordinal));
                    writer.WriteLine(",");
                });

            writer.Indent--;
            writer.Write("}");
            writer.Indent--;

            if (createTableOperation.Annotations.Any())
            {
                writer.WriteLine(",");
                writer.Write("annotations: ");
                GenerateAnnotations(createTableOperation.Annotations, writer);
            }

            writer.Write(")");

            GenerateInline(createTableOperation.PrimaryKey, writer);

            writer.WriteLine(";");
            writer.Indent--;
            writer.WriteLine();
        }

        protected override string Generate(
            IEnumerable<MigrationOperation> operations, string @namespace, string className)
        {
            Check.NotNull(operations, "operations");
            Check.NotEmpty(className, "className");

            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    WriteClassStart(
                        @namespace, className, writer, "DbMigration", designer: false,
                        namespaces: GetNamespaces(operations));

                    writer.WriteLine("public override void Up()");
                    writer.WriteLine("{");
                    writer.Indent++;

                    operations
                        .Each<dynamic>(o => Generate(o, writer));

                    writer.Indent--;
                    writer.WriteLine("}");

                    writer.WriteLine();

                    writer.WriteLine("public override void Down()");
                    writer.WriteLine("{");
                    writer.Indent++;

                    operations
                        = operations
                            .Select(o => o.Inverse)
                            .Where(o => o != null)
                            .Reverse();

                    var hasUnsupportedOperations
                        = operations.Any(o => o is NotSupportedOperation);

                    operations
                        .Where(o => !(o is NotSupportedOperation))
                        .Each<dynamic>(o => Generate(o, writer));

                    if (hasUnsupportedOperations)
                    {
                        writer.Write("throw new NotSupportedException(");
                        writer.Write(Generate("Scaffolding create or alter procedure operations is not supported in down methods."));
                        writer.WriteLine(");");
                    }

                    writer.Indent--;
                    writer.WriteLine("}");

                    WriteClassEnd(@namespace, writer);
                }

                return stringWriter.ToString();
            }
        }
    }
}
