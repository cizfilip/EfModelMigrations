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
        
        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            foreach (var property in propertiesToAdd)
            {
                //TODO: predat parametry
                yield return new AddPropertyTransformation(className, property);
            }
        }

        //TODO: Dat stringy vyjimek do resourcu
        public override void ParseParameters(string[] parameters)
        {
            if (parameters.Length < 1)
            {
                throw new ModelMigrationsException("Name of class for new properties is missing.");
            }
            if (parameters.Length < 2)
            {
                throw new ModelMigrationsException("No property to add.");
            }

            className = parameters[0];

            propertiesToAdd = ParametersParser.ParseProperties(parameters.Skip(1));
        }

        public override string GetMigrationName()
        {
            return "AddProperties" + string.Join("", propertiesToAdd.Select(p => p.Name));
        }
    }
}
