using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public interface IModelMigrationGenerator
    {
        GeneratedModelMigration GenerateMigration(string migrationId, string migrationDirectory, IEnumerable<ModelTransformation> transformations, string @namespace, string className);
    }
}
