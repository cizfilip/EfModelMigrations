using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EfModelMigrations.Commands
{
    public class CreateClassCommand : ModelMigrationsCommand
    {
        private ClassCodeModel codeModel;

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
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
                Properties = ParametersParser.ParseProperties(parameters.Skip(1))
            };

        }

        
    }
}
