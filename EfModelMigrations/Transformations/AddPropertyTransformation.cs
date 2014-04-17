using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class AddPropertyTransformation : ModelTransformation
    {
        public string ClassName { get; private set; }
        public PrimitivePropertyCodeModel Model { get; private set; }

        public AddPropertyTransformation(string className, PrimitivePropertyCodeModel model)
        {
            Check.NotEmpty(className, "className");
            Check.NotNull(model, "model");

            this.ClassName = className;
            this.Model = model;
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new AddPropertyToClassOperation(ClassName, Model);

            yield return new AddMappingInformationOperation(new AddPropertyMapping(ClassName, Model));
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.AddColumnOperation(
                builder.NewModel.GetStoreEntitySetForClass(ClassName),
                builder.NewModel.GetStoreColumnForProperty(ClassName, Model.Name)
                );
        }

        public override ModelTransformation Inverse()
        {
            return new RemovePropertyTransformation(ClassName, Model.Name, this);
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
