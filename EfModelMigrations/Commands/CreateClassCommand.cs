using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Model;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public class CreateClassCommand : ModelMigrationsCommand
    {
        private string className;
        private List<PropertyModel> properties;

        public CreateClassCommand()
        {
            properties = new List<PropertyModel>();
        }

        public override IEnumerable<ModelTransformation> GetTransformations()
        {
            yield return new CreateClassTransformation(className, properties);
        }

        public override string GetMigrationName()
        {
            return "CreateClass" + className;
        }

        //TODO: Dat stringy vyjimek do resourcu
        public override void ParseParameters(string[] parameters)
        {
            if (parameters.Length < 1)
            {
                throw new ModelMigrationsException("Name od the new class is missing.");
            }

            className = parameters[0];

            foreach (string param in parameters.Skip(1))
            {
                properties.Add(ParsePropertyModel(param));
            }
        }

        //TODO: Dat stringy vyjimek do resourcu
        private PropertyModel ParsePropertyModel(string param)
        {
            var splitted = param.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (splitted.Length != 2)
            {
                throw new ModelMigrationsException("Wrong property format, use [PropertyName]:[PropertyType], example: Name:string ");
            }

            return new PropertyModel()
            {
                Name = splitted[0],
                Type = splitted[1]
            };

        }
    }
}
