using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Transformations.Preconditions;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;

namespace EfModelMigrations.Transformations
{
    public abstract class ModelTransformation
    {
        public virtual IEnumerable<ModelTransformationPrecondition> GetPreconditions()
        {
            return Enumerable.Empty<ModelTransformationPrecondition>();
        }

        public virtual IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            return Enumerable.Empty<IModelChangeOperation>();
        }

        public virtual IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return Enumerable.Empty<MigrationOperation>(); 
        }

        //Called only by Model Migration Generator
        public abstract ModelTransformation Inverse();

        public abstract bool IsDestructiveChange { get; }
    }
}
