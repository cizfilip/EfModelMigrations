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
    //TODO: pridat moznost predat volitelne jmeno migrace do vsech commandu
    public class EmptyMigrationCommand : ModelMigrationsCommand
    {
        private string migrationName;

        public EmptyMigrationCommand(string migrationName)
        {
            if (string.IsNullOrWhiteSpace(migrationName))
            {
                throw new ModelMigrationsException(Strings.Commands_MigrationNameMissing);
            }

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
