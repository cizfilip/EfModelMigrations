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
    public class RemovePropertiesCommand : ModelMigrationsCommand
    {
        private string className;
        private IEnumerable<string> propertiesToRemoveNames;

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            foreach (var property in propertiesToRemoveNames)
            {
                yield return new RemovePropertyTransformation(className, property, 
                    new AddPropertyTransformation(className, GetPropertyModel(property, modelProvider))
                    );
            }
        }

        private PropertyCodeModel GetPropertyModel(string property, IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.GetClassCodeModel(className);
            //TODO: vracet hezci vyjimku kdyz se property nenajde
            return classModel.Properties.Single(p => string.Equals(p.Name, property, StringComparison.OrdinalIgnoreCase));
        }

        //TODO: Dat stringy vyjimek do resourcu
        public override void ParseParameters(string[] parameters)
        {
            if (parameters.Length < 1)
            {
                throw new ModelMigrationsException("Name of class for removing properties is missing.");
            }
            if (parameters.Length < 2)
            {
                throw new ModelMigrationsException("No property to remove.");
            }

            className = parameters[0];

            propertiesToRemoveNames = parameters.Skip(1);
        }

        public override string GetMigrationName()
        {
            return "RemoveProperties" + string.Join("", propertiesToRemoveNames);
        }
    }
}
