using EfModelMigrations.Infrastructure;
using EfModelMigrations.Transformations;
using System.Collections.Generic;

namespace EfModelMigrations.Commands
{
    //TODO: projit cely projekt a peclive zvazit co udelat internal a co public !!!
    public abstract class ModelMigrationsCommand
    {
        private string migrationName;
        internal string MigrationName
        {
            set
            {
                this.migrationName = value;
            }
        }


        public abstract IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider);

        protected abstract string GetDefaultMigrationName();

        public virtual string GetMigrationName()
        {
            if (string.IsNullOrWhiteSpace(migrationName))
            {
                return GetDefaultMigrationName();
            }
            return migrationName;
        }
    }
}
