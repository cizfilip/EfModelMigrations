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
        private IEnumerable<ScalarPropertyCodeModel> properties;

        //TODO: Dat stringy vyjimek do resourcu
        public CreateClassCommand(string className, string[] properties)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException("Name od the new class is missing.");
            }

            //TODO: parsovat i dalsi veci az budou hotovz lepsi parametry z powershellu
            this.className = className;
            this.properties = ParametersParser.ParseProperties(properties);
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new CreateClassTransformation(className, properties);
        }

        public override string GetMigrationName()
        {
            return "CreateClass" + className;
        }
    }
}
