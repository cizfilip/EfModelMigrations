using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.CodeModel;
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
        private ClassCodeModel codeModel;

        public override IEnumerable<ModelTransformation> GetTransformations()
        {
            yield return new CreateClassTransformation(codeModel);
        }

        public override string GetMigrationName()
        {
            return "CreateClass" + codeModel.Name;
        }

        //TODO: Dat stringy vyjimek do resourcu
        public override void ParseParameters(string[] parameters)
        {
            if (parameters.Length < 1)
            {
                throw new ModelMigrationsException("Name od the new class is missing.");
            }

            //TODO: parsovat i dalsi veci az budou hotovz lepsi parametry z powershellu
            codeModel = new ClassCodeModel()
            {
                Name = parameters[0],
                Properties = ParseProperties(parameters.Skip(1))
            };

        }

        private IEnumerable<PropertyCodeModel> ParseProperties(IEnumerable<string> parameters)
        {
            foreach (var param in parameters)
            {
                yield return ParsePropertyModel(param);
            }
        }

        //TODO: Dat stringy vyjimek do resourcu
        private PropertyCodeModel ParsePropertyModel(string param)
        {
            var splitted = param.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (splitted.Length != 2)
            {
                throw new ModelMigrationsException("Wrong property format, use [PropertyName]:[PropertyType], example: Name:string ");
            }

            return new PropertyCodeModel()
            {
                Name = splitted[0],
                Type = splitted[1]
            };

        }
    }
}
