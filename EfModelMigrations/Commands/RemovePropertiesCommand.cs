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
using EfModelMigrations.Resources;

namespace EfModelMigrations.Commands
{
    public class RemovePropertiesCommand : ModelMigrationsCommand
    {
        private string className;
        private IEnumerable<string> propertiesToRemove;

        public RemovePropertiesCommand(string className, string[] propertiesToRemove)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ModelMigrationsException(Strings.Commands_RemoveProperties_ClassNameMissing);
            }
            if (propertiesToRemove == null || propertiesToRemove.Length == 0)
            {
                throw new ModelMigrationsException(Strings.Commands_RemoveProperties_PropertiesMissing(className));
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

        private PrimitivePropertyCodeModel GetPropertyModel(string property, IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.GetClassCodeModel(className);
            
            var propertyModel = classModel.Properties.FirstOrDefault(p => p.Name.EqualsOrdinalIgnoreCase(property));

            if(propertyModel == null)
            {
                throw new ModelMigrationsException(Strings.Commands_RemoveProperties_PropertyNotFound(property, classModel.Name)); 
            }

            return propertyModel;
        }

        protected override string GetDefaultMigrationName()
        {
            return "RemoveProperties" + string.Join("", propertiesToRemove);
        }
    }
}
