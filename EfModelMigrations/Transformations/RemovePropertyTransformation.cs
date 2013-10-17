using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class RemovePropertyTransformation : ModelTransformation
    {
        private AddPropertyTransformation inverse;

        public string ClassName { get; private set; }
        public string PropertyName { get; private set; }

        public RemovePropertyTransformation(string className, string propertyName)
        {
            this.ClassName = className;
            this.PropertyName = propertyName;
        }

        public RemovePropertyTransformation(string className, string propertyName, AddPropertyTransformation inverse) 
            : this(className, propertyName)
        {
            this.inverse = inverse;
        }
       
        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.GetClassCodeModel(ClassName);
            //TODO: vracet hezci vyjimku kdyz se property nenajde
            //TODO: stejnej kod je i v RemovePropertiesCommand
            var propertyModel = classModel.Properties.Single(p => p.Name.EqualsOrdinalIgnoreCase(PropertyName));

            yield return new RemovePropertyFromClassOperation(classModel, propertyModel);
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.DropColumnOperation(ClassName, PropertyName);
        }

        public override ModelTransformation Inverse()
        {
            return inverse;
        }
    }
}
