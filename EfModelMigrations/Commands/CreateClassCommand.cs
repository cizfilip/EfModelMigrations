using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EfModelMigrations.Commands
{
    //TODO: Prijmat i dalsi parametry v prikazu (predek, implementovane interfacy, viditelnost, isAbstract, isPartial atd...)
    public class CreateClassCommand : ModelMigrationsCommand
    {
        private string className;
        private IEnumerable<PropertyCodeModel> properties;

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
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

            //TODO: parsovat i dalsi veci az budou hotovz lepsi parametry z powershellu
            className = parameters[0];
            properties = ParametersParser.ParseProperties(parameters.Skip(1));
        }

        
    }
}
