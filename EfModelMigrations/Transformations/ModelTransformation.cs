using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Mapping;
using EfModelMigrations.Operations;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;

namespace EfModelMigrations.Transformations
{
    public abstract class ModelTransformation
    {
        public virtual IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            return Enumerable.Empty<ModelChangeOperation>();
        }

        public virtual IEnumerable<IMappingInformation> GetMappingInformations(IClassModelProvider modelProvider)
        {
            return Enumerable.Empty<IMappingInformation>();
        }

        public virtual MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return null;
        }

        //Called only by Model Migration Generator
        public abstract ModelTransformation Inverse();
    }
}
