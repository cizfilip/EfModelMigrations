using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public abstract class ModelMigrationsCommand
    {
        public abstract IEnumerable<ModelTransformation> GetTransformations();

        public abstract void ParseParameters(string[] parameters);

        public abstract string GetMigrationName();
    }
}
