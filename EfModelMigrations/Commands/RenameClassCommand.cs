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
    public class RenameClassCommand : ModelMigrationsCommand
    {
        private string oldClassName;
        private string newClassName;

        //TODO: Dat stringy vyjimek do resourcu
        public RenameClassCommand(string oldClassName, string newClassName)
        {
            if (string.IsNullOrWhiteSpace(oldClassName))
            {
                throw new ModelMigrationsException("You must specify old class name.");
            }

            if (string.IsNullOrWhiteSpace(newClassName))
            {
                throw new ModelMigrationsException("You must specify new class name.");
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
