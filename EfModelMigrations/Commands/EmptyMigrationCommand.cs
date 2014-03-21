using EfModelMigrations.Infrastructure;
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
        private string migrationName;

        public EmptyMigrationCommand(string migrationName)
        {
            this.migrationName = migrationName;
        }


        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            return Enumerable.Empty<ModelTransformation>();
        }

        public override string GetMigrationName()
        {
            return migrationName;
        }
    }
}
