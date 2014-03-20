using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure.Generators.Templates;
using Microsoft.CSharp.RuntimeBinder;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.CodeModel;
using System.Data.Entity.Core.Metadata.Edm;


namespace EfModelMigrations.Infrastructure.Generators
{
    public class CSharpModelMigrationGenerator : ModelMigrationGeneratorBase
    {
        protected static readonly string Indent = "    ";

        public override GeneratedModelMigration GenerateMigration(string migrationId, IEnumerable<ModelTransformation> transformations, string @namespace, string className)
        {
            string upMethodBody = GenerateMethodBody(transformations);

            string downMethodBody = GenerateMethodBody(transformations.Select(t => t.Inverse()).Where(t => t != null).Reverse());

            var template = new ModelMigrationTemplate()
            {
                MigrationId = migrationId,
                Namespace = @namespace,
                ClassName = className,
                Imports = GetImportNamespaces(),
                UpMethod = upMethodBody,
                DownMethod = downMethodBody
            };

            var generatedMigration = new GeneratedModelMigration()
            {
                SourceCode = template.TransformText()
            };

            return generatedMigration;
        }

        protected virtual string GenerateMethodBody(IEnumerable<ModelTransformation> transformations)
        {
            StringBuilder builder = new StringBuilder();

            bool prependLine = false;

            foreach (dynamic transformation in transformations)
            {
                try
                {
                    if(prependLine)
                    {
                        builder.AppendLine();
                    }
                    prependLine = true;
                    Generate(transformation, builder);
                }
                catch (RuntimeBinderException e)
                {
                    //TODO: string do resourcu
                    throw new ModelMigrationsException(string.Format("Cannot generate migration code for model transformation {0}. Generator implementation is missing.", transformation.GetType().Name), e);
                }
            }
            return builder.ToString();
        }

        protected virtual void Generate(CreateClassTransformation transformation, StringBuilder builder)
        {
            builder.AppendLine("this.CreateClass(");
            AppendIndent(builder);
            builder.Append(QuoteString(transformation.Name))
                .AppendLine(",");
            AppendIndent(builder);
            builder.AppendLine("p => new");
            AppendIndent(builder, 2);
            builder.AppendLine("{");
                        

            
            foreach (var property in transformation.Properties)
            {
                AppendIndent(builder, 3);
                builder.Append(property.Name)
                    .Append(" = ")
                    .Append("p.");

                Generate(property, builder);

                builder.AppendLine(",");
            }

            AppendIndent(builder, 2);
            builder.Append("});");
        }

        protected virtual void Generate(RemoveClassTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.RemoveClass(");
            builder.Append(QuoteString(transformation.Name));
            builder.Append(");");
        }

        protected virtual void Generate(AddPropertyTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.AddProperty(")
                .Append(QuoteString(transformation.ClassName))
                .Append(", ")
                .Append(QuoteString(transformation.Model.Name))
                .Append(", ")
                .Append("p => p.");

            Generate(transformation.Model, builder);
            
            builder.Append(");");
        }

        protected virtual void Generate(RemovePropertyTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.RemoveProperty(");
            builder.Append(QuoteString(transformation.ClassName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.Name));
            builder.Append(");");
        }

        //TODO: generovat pomoci named parametru aby vysledek byl: this.RenameClass(oldName: name, newName: name)
        protected virtual void Generate(RenameClassTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.RenameClass(");
            builder.Append(QuoteString(transformation.OldName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.NewName));
            builder.Append(");");
        }

        //TODO: generovat pomoci named parametru aby vysledek byl: this.RenameProperty(className: name, oldName: name, newName: name)
        protected virtual void Generate(RenamePropertyTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.RenameProperty(");
            builder.Append(QuoteString(transformation.ClassName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.OldName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.NewName));
            builder.Append(");");
        }

        protected virtual void Generate(ExtractComplexTypeTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.ExtractComplexType(");
            builder.Append(QuoteString(transformation.ClassName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.ComplexTypeName));
            builder.Append(", ");
            builder.Append("new string[] { ");
            foreach (var property in transformation.PropertiesToExtract)
            {
                builder.Append(QuoteString(property));
                builder.Append(", ");
            }

            builder.Append("}");
            builder.Append(");");
        }

        protected virtual void Generate(JoinComplexTypeTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.JoinComplexType(");
            builder.Append(QuoteString(transformation.ComplexTypeName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.ClassName));
            builder.Append(");");
        }


        protected virtual void Generate(ScalarProperty property, StringBuilder builder)
        {
            builder.Append(TranslatePrimitiveTypeToBuilderMethodName(property.Type))
                .Append("(");

            bool includeComma = false;
            if(property.Visibility.HasValue)
            {
                builder.Append("visibility: ")
                    .Append(TranslateCodeModelVisbility(property.Visibility.Value));
                includeComma = true;
            }

            if(property.IsVirtual.HasValue)
            {
                if(includeComma)
                {
                    builder.Append(", ");
                }
                
                builder.Append("isVirtual: ")
                    .Append(property.IsVirtual.Value.ToString());
            }

            builder.Append(")");
        }

        private string TranslateCodeModelVisbility(CodeModelVisibility codeModelVisibility)
        {
            switch (codeModelVisibility)
            {
                case CodeModelVisibility.Public:
                    return "CodeModelVisibility.Public";
                case CodeModelVisibility.Private:
                    return "CodeModelVisibility.Private";
                case CodeModelVisibility.Protected:
                    return "CodeModelVisibility.Protected";
                case CodeModelVisibility.Internal:
                    return "CodeModelVisibility.Internal";
                case CodeModelVisibility.ProtectedInternal:
                    return "CodeModelVisibility.ProtectedInternal";
                default:
                    throw new InvalidOperationException("Invalid CodeModelVisibility."); //TODO: string do resourcu
            }
        }

        protected virtual string TranslatePrimitiveTypeToBuilderMethodName(PrimitiveTypeKind type)
        {
            switch (type)
            {
                case PrimitiveTypeKind.Binary:
                    return "Binary";
                case PrimitiveTypeKind.Boolean:
                    return "Boolean";
                case PrimitiveTypeKind.Byte:
                    return "Byte";
                case PrimitiveTypeKind.DateTime:
                    return "DateTime";
                case PrimitiveTypeKind.Time:
                    return "Time";
                case PrimitiveTypeKind.DateTimeOffset:
                    return "DateTimeOffset";
                case PrimitiveTypeKind.Decimal:
                    return "Decimal";
                case PrimitiveTypeKind.Double:
                    return "Double";
                case PrimitiveTypeKind.Geography:
                case PrimitiveTypeKind.GeographyPoint:
                case PrimitiveTypeKind.GeographyLineString:
                case PrimitiveTypeKind.GeographyPolygon:
                case PrimitiveTypeKind.GeographyMultiPoint:
                case PrimitiveTypeKind.GeographyMultiLineString:
                case PrimitiveTypeKind.GeographyMultiPolygon:
                case PrimitiveTypeKind.GeographyCollection:
                    return "Geography"; 
                case PrimitiveTypeKind.Geometry:
                case PrimitiveTypeKind.GeometryPoint:
                case PrimitiveTypeKind.GeometryLineString:
                case PrimitiveTypeKind.GeometryPolygon:
                case PrimitiveTypeKind.GeometryMultiPoint:
                case PrimitiveTypeKind.GeometryMultiLineString:
                case PrimitiveTypeKind.GeometryMultiPolygon:
                case PrimitiveTypeKind.GeometryCollection:
                    return "Geometry";
                case PrimitiveTypeKind.Guid:
                    return "Guid";
                case PrimitiveTypeKind.Single:
                    return "Single";
                case PrimitiveTypeKind.SByte:
                    throw new InvalidOperationException("PrimitiveTypeKind.SByte not supported yet."); //TODO: string do resourcu
                case PrimitiveTypeKind.Int16:
                    return "Short";
                case PrimitiveTypeKind.Int32:
                    return "Int";
                case PrimitiveTypeKind.Int64:
                    return "Long";
                case PrimitiveTypeKind.String:
                    return "String";
                default:
                    throw new InvalidOperationException("Invalid PrimitiveTypeKind."); //TODO: string do resourcu
            }
        }

        protected virtual string QuoteString(string str)
        {
            return "\"" + str + "\"";
        }

        //TODO: refaktorovat a udelat si tridu IndentedWriter po vzore EF a tu pak pouzivat ve vsech generatorech (anebo pouzivat primo tu z EF...)
        protected virtual void AppendIndent(StringBuilder builder, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                builder.Append(Indent);
            }
        }
    }
}
