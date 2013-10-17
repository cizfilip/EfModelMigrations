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
    public class RenamePropertyCommand : ModelMigrationsCommand
    {
        private string className;
        private string oldPropertyName;
        private string newPropertyName;

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new RenamePropertyTransformation(className, oldPropertyName, newPropertyName);
        }

        //TODO: Dat stringy vyjimek do resourcu
        public override void ParseParameters(string[] parameters)
        {
            if (parameters.Length != 3)
            {
                throw new ModelMigrationsException("You must specify class name old property name and new property name.");
            }

            className = parameters[0];
            oldPropertyName = parameters[1];
            newPropertyName = parameters[2];
        }

        public override string GetMigrationName()
        {
            return "RenameProperty" + oldPropertyName + "In" + className;
        }
    }
}
