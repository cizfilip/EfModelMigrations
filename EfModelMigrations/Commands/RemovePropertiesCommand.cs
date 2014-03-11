using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations;
using EfModelMigrations.Extensions;
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
        private IEnumerable<string> propertiesToRemove;

        //TODO: Dat stringy vyjimek do resourcu
        public RemovePropertiesCommand(string className, string[] propertiesToRemove)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException("Name of class for removing properties is missing.");
            }
            if (propertiesToRemove == null || propertiesToRemove.Length == 0)
            {
                throw new ModelMigrationsException("No property to remove.");
            }

            this.className = className;
            this.propertiesToRemove = propertiesToRemove;
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            foreach (var property in propertiesToRemove)
            {
                yield return new RemovePropertyTransformation(className, property, 
                    new AddPropertyTransformation(className, GetPropertyModel(property, modelProvider))
                    );
            }
        }

        private ScalarProperty GetPropertyModel(string property, IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.GetClassCodeModel(className);
            //TODO: vracet hezci vyjimku kdyz se property nenajde
            return classModel.Properties.Single(p => p.Name.EqualsOrdinalIgnoreCase(property));
        }

        public override string GetMigrationName()
        {
            return "RemoveProperties" + string.Join("", propertiesToRemove);
        }
    }
}
