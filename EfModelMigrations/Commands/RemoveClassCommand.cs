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
    public class RemoveClassCommand : ModelMigrationsCommand
    {
        private string className;

        public RemoveClassCommand(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException(Strings.Commands_RemoveClass_ClassNameMissing);
            }

            this.className = className;
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.GetClassCodeModel(className);

            yield return new RemoveClassTransformation(className,
                new CreateClassTransformation(classModel.ToClassModel(), classModel.Properties)
                );
        }

        public override string GetMigrationName()
        {
            return "RemoveClass" + className;
        }
    }
}
