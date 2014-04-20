using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Resources;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public class EmptyMigrationCommand : ModelMigrationsCommand
    {
        public EmptyMigrationCommand()
        {
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            return Enumerable.Empty<ModelTransformation>();
        }

        protected override string GetDefaultMigrationName()
        {
            throw new InvalidOperationException(Strings.Commands_MigrationNameMissing);
        }
    }
}
