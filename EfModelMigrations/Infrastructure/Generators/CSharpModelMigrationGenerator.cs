using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure.Generators.Templates;


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
                //TODO: Co kdyz prijde podtyp ModelTransformation pro ktery neni definovana metoda Generate? - nejak osetrit tuhle vyjimku
                Generate(transformation, builder);
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

        protected string QuoteString(string str)
        {
            return "\"" + str + "\"";
        }
    }
}
