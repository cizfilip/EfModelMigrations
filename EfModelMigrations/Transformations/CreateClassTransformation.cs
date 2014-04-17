using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using EfModelMigrations.Transformations.Preconditions;

namespace EfModelMigrations.Transformations
{
    public class CreateClassTransformation : ModelTransformation
    {
        public ClassModel Model { get; private set; }
        public IEnumerable<PrimitivePropertyCodeModel> Properties { get; private set; }
        public string[] PrimaryKeys { get; private set; }

        public CreateClassTransformation(ClassModel model, IEnumerable<PrimitivePropertyCodeModel> properties, string[] primaryKeys = null)
        {
            Check.NotNull(model, "model");
            Check.NotNullOrEmpty(properties, "properties");

            this.Model = model;
            this.Properties = properties;
            this.PrimaryKeys = primaryKeys;
        }

        public override IEnumerable<ModelTransformationPrecondition> GetPreconditions()
        {
            //TODO: pridat preconditions u vsech transformaci!
            //TODO: udelat precondition co validuje spravnost pridavanych property - momentalne asi jenom v pripade ze je property enum tak enum musi existovat...
            yield return new ClassNotExistsInModelPrecondition(Model.Name);
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new CreateEmptyClassOperation(Model.Name, Model.Visibility);
            yield return new AddMappingInformationOperation(new AddClassMapping(Model, PrimaryKeys));

            foreach (var property in Properties)
            {
                yield return new AddPropertyToClassOperation(Model.Name, property);
                yield return new AddMappingInformationOperation(new AddPropertyMapping(Model.Name, property));
            }

            yield return new AddDbSetPropertyOperation(Model.Name);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.CreateTableOperation(
                    builder.NewModel.GetStoreEntitySetForClass(Model.Name)
                );
        }
        
        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(Model.Name, this);
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
