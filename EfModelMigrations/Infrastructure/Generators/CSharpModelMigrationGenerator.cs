﻿using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure.Generators.Templates;
using Microsoft.CSharp.RuntimeBinder;
using EfModelMigrations.Exceptions;


namespace EfModelMigrations.Infrastructure.Generators
{
    public class CSharpModelMigrationGenerator : ModelMigrationGeneratorBase
    {
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

            foreach (dynamic transformation in transformations)
            {
                try
                {
                    Generate(transformation, builder);
                }
                catch (RuntimeBinderException e)
                {
                    //TODO: string do resourcu
                    throw new ModelMigrationsException(string.Format("Cannot generate migration code for model transformation {0}. Generator implementation is missing.", transformation.GetType().Name), e);
                }
            }

            //remove last new line
            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }

            return builder.ToString();
        }

        protected virtual void Generate(CreateClassTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.CreateClass(");
            builder.Append(QuoteString(transformation.Name));
            builder.AppendLine(", new {");

            string indent = "    ";
            foreach (var property in transformation.Properties)
            {
                builder.Append(indent);
                builder.Append(property.Name);
                builder.Append(" = ");
                builder.Append(QuoteString(property.Type));
                builder.AppendLine(",");
            }

            builder.AppendLine("});");
        }

        protected virtual void Generate(RemoveClassTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.RemoveClass(");
            builder.Append(QuoteString(transformation.Name));
            builder.AppendLine(");");
        }

        protected virtual void Generate(AddPropertyTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.AddProperty(");
            builder.Append(QuoteString(transformation.ClassName));
            builder.AppendLine(", new {");

            string indent = "    ";

            builder.Append(indent);
            builder.Append(transformation.Model.Name);
            builder.Append(" = ");
            builder.Append(QuoteString(transformation.Model.Type));
            builder.AppendLine();

            builder.AppendLine("});");
        }

        protected virtual void Generate(RemovePropertyTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.RemoveProperty(");
            builder.Append(QuoteString(transformation.ClassName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.Name));
            builder.AppendLine(");");
        }

        //TODO: generovat pomoci named parametru aby vysledek byl: this.RenameClass(oldName: name, newName: name)
        protected virtual void Generate(RenameClassTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.RenameClass(");
            builder.Append(QuoteString(transformation.OldName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.NewName));
            builder.AppendLine(");");
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
            builder.AppendLine(");");
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
            builder.AppendLine(");");
        }

        protected virtual void Generate(JoinComplexTypeTransformation transformation, StringBuilder builder)
        {
            builder.Append("this.JoinComplexType(");
            builder.Append(QuoteString(transformation.ComplexTypeName));
            builder.Append(", ");
            builder.Append(QuoteString(transformation.ClassName));
            builder.AppendLine(");");
        }


        protected string QuoteString(string str)
        {
            return "\"" + str + "\"";
        }
    }
}
