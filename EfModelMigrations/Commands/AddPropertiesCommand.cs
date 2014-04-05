using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    //TODO: vyhodit a podporovat jen command pro pridani jedne property !!!
    public class AddPropertiesCommand : ModelMigrationsCommand
    {
        private string className;
        private string[] propertiesToAdd;

        //TODO: Dat stringy vyjimek do resourcu
        public AddPropertiesCommand(string className, string[] propertiesToAdd)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException("Name of class for new properties is missing.");
            }
            if (propertiesToAdd == null || propertiesToAdd.Length == 0)
            {
                throw new ModelMigrationsException("No property to add.");
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

        //TODO: predelat jmeno
        public override string GetMigrationName()
        {
            return "AddPropertiesToClass" + className; // string.Join("", propertiesToAdd.Select(p => p.Name));
        }
    }
}
