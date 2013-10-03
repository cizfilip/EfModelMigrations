using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public abstract class ModelTransformation
    {
        public abstract IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider);

        public abstract MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder);

        //Called only by Model Migration Generator
        public abstract ModelTransformation Inverse();
    }
}
