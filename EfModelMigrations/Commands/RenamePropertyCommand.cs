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

        //TODO: Dat stringy vyjimek do resourcu
        public RenamePropertyCommand(string className, string oldPropertyName, string newPropertyName)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException("You must specify class name. old property name and new property name.");
            }

            if (string.IsNullOrWhiteSpace(oldPropertyName))
            {
                throw new ModelMigrationsException("You must specify old property name.");
            }

            if (string.IsNullOrWhiteSpace(newPropertyName))
            {
                throw new ModelMigrationsException("You must specify new property name.");
            }

            this.className = className;
            this.oldPropertyName = oldPropertyName;
            this.newPropertyName = newPropertyName;
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new RenamePropertyTransformation(className, oldPropertyName, newPropertyName);
        }

        
        public override string GetMigrationName()
        {
            return "RenameProperty" + oldPropertyName + "In" + className;
        }
    }
}
