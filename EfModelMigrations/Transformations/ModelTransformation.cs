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
        //TODO: mozna odstranit parametr modelProvider
        public virtual IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            return Enumerable.Empty<IModelChangeOperation>();
        }

        public virtual IEnumerable<IMappingInformation> GetMappingInformationsToAdd(IClassModelProvider modelProvider)
        {
            return Enumerable.Empty<IMappingInformation>();
        }

        public virtual IEnumerable<IMappingInformation> GetMappingInformationsToRemove(IClassModelProvider modelProvider)
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
