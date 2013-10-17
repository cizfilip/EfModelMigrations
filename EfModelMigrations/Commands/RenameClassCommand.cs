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


        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new RenameClassTransformation(oldClassName, newClassName);
        }

        //TODO: Dat stringy vyjimek do resourcu
        public override void ParseParameters(string[] parameters)
        {
            if (parameters.Length != 2)
            {
                throw new ModelMigrationsException("You must specify old class name and new class name.");
            }

            oldClassName = parameters[0];
            newClassName = parameters[1];
        }

        public override string GetMigrationName()
        {
            return "RenameClass" + oldClassName;
        }
    }

    
}
