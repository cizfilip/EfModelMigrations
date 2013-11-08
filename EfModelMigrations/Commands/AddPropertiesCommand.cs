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
    public class AddPropertiesCommand : ModelMigrationsCommand
    {
        private string className;
        private IEnumerable<PropertyCodeModel> propertiesToAdd;

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
            this.propertiesToAdd = ParametersParser.ParseProperties(propertiesToAdd);
        }
        
        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            foreach (var property in propertiesToAdd)
            {
                //TODO: predat parametry
                yield return new AddPropertyTransformation(className, property);
            }
        }

        public override string GetMigrationName()
        {
            return "AddProperties" + string.Join("", propertiesToAdd.Select(p => p.Name));
        }
    }
}
