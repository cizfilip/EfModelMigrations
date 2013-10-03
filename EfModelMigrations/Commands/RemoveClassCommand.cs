using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public class RemoveClassCommand : ModelMigrationsCommand
    {
        private string className;


        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new RemoveClassTransformation(className,
                new CreateClassTransformation(modelProvider.GetClassCodeModel(className))
                );
        }

        //TODO: Dat stringy vyjimek do resourcu
        public override void ParseParameters(string[] parameters)
        {
            if (parameters.Length < 1)
            {
                throw new ModelMigrationsException("Name of class to remove is missing.");
            }

            className = parameters[0];
        }

        public override string GetMigrationName()
        {
            return "RemoveClass" + className;
        }
    }
}
