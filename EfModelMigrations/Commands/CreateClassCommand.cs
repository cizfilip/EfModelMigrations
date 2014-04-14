using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EfModelMigrations.Commands
{
    //TODO: Prijmat i dalsi parametry v prikazu (predek, implementovane interfacy, viditelnost, isAbstract, isPartial atd...)
    public class CreateClassCommand : ModelMigrationsCommand
    {
        private string className;
        private string[] properties;
        private string visibility;
        private string tableName;
        private string schema;

        //TODO: Dat stringy vyjimek do resourcu
        public CreateClassCommand(string className, string visibility, string tableName, string schema, string[] properties)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException("Name od the new class is missing.");
            }
            if (properties == null || properties.Length == 0)
            {
                throw new ModelMigrationsException("Properties for the new class is missing.");
            }

            this.className = className;
            this.properties = properties;
            this.visibility = visibility;
            this.tableName = tableName;
            this.schema = schema;
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            var parameterParser = new ParametersParser(modelProvider);

            CodeModelVisibility? classVisibility = null;
            if(!string.IsNullOrWhiteSpace(this.visibility))
            {
                classVisibility = parameterParser.ParseVisibility(this.visibility);
            }

            TableName table = null;
            if(!string.IsNullOrWhiteSpace(tableName))
            {
                table = new TableName(tableName, schema);
            }

            yield return new CreateClassTransformation(new ClassModel(className, table, classVisibility), parameterParser.ParseProperties(properties));
        }

        public override string GetMigrationName()
        {
            return "CreateClass" + className;
        }
    }
}
