using EfModelMigrations.Infrastructure;
using EfModelMigrations.Transformations;
using System.Collections.Generic;

namespace EfModelMigrations.Commands
{
    //TODO: projit cely projekt a peclive zvazit co udelat internal a co public !!!
    public abstract class ModelMigrationsCommand
    {
        public abstract IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider);

        public abstract string GetMigrationName();
    }
}
