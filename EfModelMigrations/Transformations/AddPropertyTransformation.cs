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
        public PropertyCodeModel Model { get; private set; }

        public AddPropertyTransformation(string className, PropertyCodeModel model)
        {
            this.ClassName = className;
            this.Model = model;
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new AddPropertyToClassOperation(ClassName, Model);
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.AddColumnOperation(ClassName, Model.Name);
        }

        public override ModelTransformation Inverse()
        {
            return new RemovePropertyTransformation(ClassName, Model.Name, this);
        }
    }
}
