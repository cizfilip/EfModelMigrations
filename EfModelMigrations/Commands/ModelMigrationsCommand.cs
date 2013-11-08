using EfModelMigrations.Infrastructure;
using EfModelMigrations.Transformations;
using System.Collections.Generic;

namespace EfModelMigrations.Commands
{
    public abstract class ModelMigrationsCommand
    {
        public abstract IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider);

        public abstract string GetMigrationName();
    }
}
