using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Resources;
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

        public RenamePropertyCommand(string className, 
            string oldPropertyName, 
            string newPropertyName)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException(Strings.Commands_RenameProperty_ClassNameMissing);
            }

            if (string.IsNullOrWhiteSpace(oldPropertyName))
            {
                throw new ModelMigrationsException(Strings.Commands_RenameProperty_OldPropertyMissing);
            }

            if (string.IsNullOrWhiteSpace(newPropertyName))
            {
                throw new ModelMigrationsException(Strings.Commands_RenameProperty_NewPropertyMissing);
            }

            this.className = className;
            this.oldPropertyName = oldPropertyName;
            this.newPropertyName = newPropertyName;
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new RenamePropertyTransformation(className, oldPropertyName, newPropertyName);
        }

        
        protected override string GetDefaultMigrationName()
        {
            return "RenameProperty" + oldPropertyName + "In" + className;
        }
    }
}
