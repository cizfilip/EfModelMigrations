using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class AddPropertyTransformation : ModelTransformation
    {
        public string ClassName { get; private set; }
        public PropertyCodeModel PropertyModel { get; private set; }

        public AddPropertyTransformation(string className, PropertyCodeModel propertyModel)
        {
            this.ClassName = className;
            this.PropertyModel = propertyModel;
        }

        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new AddPropertyToClassOperation(modelProvider.GetClassCodeModel(ClassName), PropertyModel);
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.AddColumnOperation(ClassName, PropertyModel.Name);
        }

        public override ModelTransformation Inverse()
        {
            return new RemovePropertyTransformation(ClassName, PropertyModel.Name);
        }
    }
}
