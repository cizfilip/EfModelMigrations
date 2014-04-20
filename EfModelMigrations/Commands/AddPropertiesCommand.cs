using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Resources;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public class AddPropertiesCommand : ModelMigrationsCommand
    {
        private string className;
        private string[] propertiesToAdd;

        //TODO: ve validaci u commandu vyhazovat tuhle vyjimku???
        public AddPropertiesCommand(string className, string[] propertiesToAdd)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException(Strings.Commands_AddProperties_ClassNameMissing);
            }
            if (propertiesToAdd == null || propertiesToAdd.Length == 0)
            {
                throw new ModelMigrationsException(Strings.Commands_AddProperties_NoProperties(className));
            }

            this.className = className;
            this.propertiesToAdd = propertiesToAdd;
        }
        
        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            var parameterParser = new ParametersParser(modelProvider);

            foreach (var property in propertiesToAdd)
            {
                yield return new AddPropertyTransformation(className, parameterParser.ParseProperty(property));
            }
        }

        protected override string GetDefaultMigrationName()
        {
            return "AddPropertiesToClass" + className;
        }
    }
}
