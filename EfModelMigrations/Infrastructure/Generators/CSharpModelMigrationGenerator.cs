﻿using EfModelMigrations.Transformations;
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
using EfModelMigrations.Resources;
using EfModelMigrations.Transformations.Model;


namespace EfModelMigrations.Infrastructure.Generators
{
    public class CSharpModelMigrationGenerator : ModelMigrationGeneratorBase
    {
        protected static readonly string Indent = "    ";
        public override GeneratedModelMigration GenerateMigration(string migrationId, string migrationDirectory, IEnumerable<ModelTransformation> transformations, string @namespace, string className)
        {
            string upMethodBody = GenerateMethodBody(transformations) ?? "";

            string downMethodBody = GenerateMethodBody(transformations.Select(t => t.Inverse()).Where(t => t != null).Reverse()) ?? "";

            var template = new ModelMigrationTemplate()
            {
                MigrationId = migrationId,
                Namespace = @namespace,
                ClassName = className,
                Imports = GetImportNamespaces(),
                UpMethod = upMethodBody,
                DownMethod = downMethodBody
            };

            return new GeneratedModelMigration(migrationId, string.Concat(@namespace, ".", className), migrationDirectory, template.TransformText(), upMethodBody, downMethodBody);
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
                    throw new ModelMigrationsException(Strings.ModelMigrationGenerator_ImplementationMissing(transformation.GetType().Name), e);
                }
            }
            return builder.ToString();
        }

        protected virtual void Generate(CreateClassTransformation transformation, StringBuilder builder)
        {
            builder.AppendLine("Model.CreateClass(");
            AppendIndent(builder);
            GenerateClassModel(transformation.Model, builder);

            builder.Append(", ");
            builder.AppendLine();
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

                GenerateProperty(property, builder);

                builder.AppendLine(",");
            }

            AppendIndent(builder, 2);
            builder.Append("});");
        }

        protected virtual void GenerateClassModel(ClassModel model, StringBuilder builder)
        {
            builder.Append("c => c.Class(");
            builder.Append(QuoteString(model.Name));

            if (model.Visibility.HasValue || model.TableName != null)
            {
                if (model.Visibility.HasValue)
                {
                    builder.Append(", ")
                        .Append("visibility: ")
                        .Append(TranslateCodeModelVisbility(model.Visibility.Value));
                }

                if (model.TableName != null)
                {
                    builder.Append(", ")
                        .Append("tableName: ")
                        .Append(QuoteString(model.TableName.Table));

                    if (!string.IsNullOrWhiteSpace(model.TableName.Schema))
                    {
                        builder.Append(", ")
                            .Append("schema: ")
                            .Append(QuoteString(model.TableName.Schema));
                    }
                }
            }
            builder.Append(")");
        }

        protected virtual void Generate(RemoveClassTransformation transformation, StringBuilder builder)
        {
            builder.Append("Model.RemoveClass(");
            builder.Append(QuoteString(transformation.Name));
            builder.Append(");");
        }

        protected virtual void Generate(ExtractClassTransformation transformation, StringBuilder builder)
        {
            builder.Append("Model.ExtractClass(")
                .Append(QuoteString(transformation.FromClass))
                .Append(", ")
                .Append("new[] { ")
                .Append(string.Join(", ", transformation.Properties.Select(p => QuoteString(p))))
                .Append(" }, ");
            GenerateClassModel(transformation.NewClass, builder);
            builder.Append(");");
        }

        protected virtual void Generate(MergeClassesTransformation transformation, StringBuilder builder)
        {
            builder.Append("Model.MergeClasses(")
                .Append(QuoteString(transformation.Principal.ClassName))
                .Append(", ");
            
            GenerateSimpleAssociationEnd(transformation.Principal, builder);

            builder.Append(", ")
                .Append(QuoteString(transformation.Dependent.ClassName))
                .Append(", ");

            GenerateSimpleAssociationEnd(transformation.Dependent, builder);

            builder.Append(", new[] { ")
                .Append(string.Join(", ", transformation.PropertiesToMerge.Select(p => QuoteString(p))))
                .Append(" });");
        }

        
        protected virtual void GenerateSimpleAssociationEnd(SimpleAssociationEnd associationEnd, StringBuilder builder, string noNavPropText = "null")
        {
            if (associationEnd.HasNavigationPropertyName)
            {
                builder.Append(QuoteString(associationEnd.NavigationPropertyName));
            }
            else
            {
                builder.Append(noNavPropText);
            }
        }

        protected virtual void Generate(AddPropertyTransformation transformation, StringBuilder builder)
        {
            builder.Append("Model.AddProperty(")
                .Append(QuoteString(transformation.ClassName))
                .Append(", ")
                .Append(QuoteString(transformation.Model.Name))
                .Append(", ")
                .Append("p => p.");

            GenerateProperty(transformation.Model, builder);
            
            builder.Append(");");
        }

        protected virtual void Generate(RemovePropertyTransformation transformation, StringBuilder builder)
        {
            builder.Append("Model.RemoveProperty(");
            builder.Append(QuoteString(transformation.ClassName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.Name));
            builder.Append(");");
        }

        //TODO: generovat pomoci named parametru aby vysledek byl: Model.RenameClass(oldName: name, newName: name)
        protected virtual void Generate(RenameClassTransformation transformation, StringBuilder builder)
        {
            builder.Append("Model.RenameClass(");
            builder.Append(QuoteString(transformation.OldName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.NewName));
            builder.Append(");");
        }

        //TODO: generovat pomoci named parametru aby vysledek byl: Model.RenameProperty(className: name, oldName: name, newName: name)
        protected virtual void Generate(RenamePropertyTransformation transformation, StringBuilder builder)
        {
            builder.Append("Model.RenameProperty(");
            builder.Append(QuoteString(transformation.ClassName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.OldName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.NewName));
            builder.Append(");");
        }

        protected virtual void GenerateProperty(PrimitivePropertyCodeModel property, StringBuilder builder)
        {
            try
            {
                dynamic prop = property;
                GenerateProperty(prop, builder);
            }
            catch (RuntimeBinderException e)
            {
                throw new ModelMigrationsException(Strings.ModelMigrationGenerator_PropertyImplementationMissing(property.Name), e);
            }
        }

        protected virtual void GenerateProperty(ScalarPropertyCodeModel property, StringBuilder builder)
        {
            builder.Append(TranslatePrimitiveTypeToBuilderMethodName(property.Type))
                .Append("(");
            bool generateTypeNullability = property.IsTypeNullable && !ScalarPropertyCodeModel.NullablePrimitiveTypes.Contains(property.Type);
            GeneratePrimitivePropertyProperties(property, generateTypeNullability, builder);
            builder.Append(")");
                        
            GenerateColumnInfo(property.Column, builder);
        }
        protected virtual void GenerateProperty(EnumPropertyCodeModel property, StringBuilder builder)
        {
            builder.Append(QuoteString(property.EnumType))
                .Append("(");
            GeneratePrimitivePropertyProperties(property, property.IsTypeNullable, builder);
            builder.Append(")");
            GenerateColumnInfo(property.Column, builder);
        }

        protected virtual void GenerateColumnInfo(ColumnInfo column, StringBuilder builder)
        {
            if(column.IsSomethingSpecified())
            {
                throw new NotImplementedException(); // Commands can not provide any column info...
            }
        }

        protected virtual void GeneratePrimitivePropertyProperties(PrimitivePropertyCodeModel property, bool isTypeNullable, StringBuilder builder)
        {
            bool includeComma = false;
            if (isTypeNullable)
            {
                builder.Append("isNullable: ")
                    .Append("true");
                includeComma = true;
            }

            if (property.Visibility.HasValue)
            {
                builder.Append("visibility: ")
                    .Append(TranslateCodeModelVisbility(property.Visibility.Value));
                includeComma = true;
            }

            if (property.IsVirtual.HasValue)
            {
                if (includeComma)
                {
                    builder.Append(", ");
                }

                builder.Append("isVirtual: ")
                    .Append(property.IsVirtual.Value.ToString().ToLowerInvariant());
                includeComma = true;
            }

            if (property.IsSetterPrivate.HasValue)
            {
                if (includeComma)
                {
                    builder.Append(", ");
                }

                builder.Append("isSetterPrivate: ")
                    .Append(property.IsSetterPrivate.Value.ToString().ToLowerInvariant());
                includeComma = true;
            }
        }

        protected virtual string TranslateCodeModelVisbility(CodeModelVisibility codeModelVisibility)
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
                    throw new InvalidOperationException(Strings.CodeModelVisibilityInvalid);
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
                    throw new InvalidOperationException(Strings.ModelMigrationGenerator_SBytePropertyNotSupported);
                case PrimitiveTypeKind.Int16:
                    return "Short";
                case PrimitiveTypeKind.Int32:
                    return "Int";
                case PrimitiveTypeKind.Int64:
                    return "Long";
                case PrimitiveTypeKind.String:
                    return "String";
                default:
                    throw new InvalidOperationException(Strings.PrimitiveTypeKindInvalid);
            }
        }

        protected virtual string QuoteString(string str)
        {
            return "\"" + str + "\"";
        }

        protected virtual void AppendIndent(StringBuilder builder, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                builder.Append(Indent);
            }
        }
    }
}
