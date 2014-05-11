using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Preconditions;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class RemoveClassTransformation : TransformationWithInverse
    {
        public string Name { get; private set; }
            
        public RemoveClassTransformation(string name, CreateClassTransformation inverse) : base(inverse)
        {
            Check.NotEmpty(name, "name");

            this.Name = name;
        }

        public RemoveClassTransformation(string name)
            : this(name, null)
        {
        }

        public override IEnumerable<ModelTransformationPrecondition> GetPreconditions()
        {
            yield return new ClassExistsInModelPrecondition(Name);
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new RemoveMappingInformationOperation(new RemoveClassMapping(Name));
            yield return new RemoveClassOperation(Name);

            yield return new RemoveDbSetPropertyOperation(Name);
        }
        
        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.DropTableOperation(
                    builder.OldModel.GetStoreEntitySetForClass(Name)
                );
        }

        public override bool IsDestructiveChange
        {
            get { return true; }
        }
    }
}
