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
    public class RenameClassCommand : ModelMigrationsCommand
    {
        private string oldClassName;
        private string newClassName;

        public RenameClassCommand(string oldClassName, string newClassName)
        {
            if (string.IsNullOrWhiteSpace(oldClassName))
            {
                throw new ModelMigrationsException(Strings.Commands_RenameClass_OldClassNameMissing);
            }

            if (string.IsNullOrWhiteSpace(newClassName))
            {
                throw new ModelMigrationsException(Strings.Commands_RenameClass_NewClassNameMissing);
            }

            this.oldClassName =  oldClassName;
            this.newClassName = newClassName;
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new RenameClassTransformation(oldClassName, newClassName);
        }

        public override string GetMigrationName()
        {
            return "RenameClass" + oldClassName;
        }
    }

    
}
