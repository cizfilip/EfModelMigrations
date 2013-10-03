using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public class ExtendedCSharpMigrationCodeGenerator : CSharpMigrationCodeGenerator
    {
        private IEnumerable<MigrationOperation> newOperations;

        public ExtendedCSharpMigrationCodeGenerator(IEnumerable<MigrationOperation> operations)
        {
            this.newOperations = operations;
        }

        public override ScaffoldedMigration Generate(string migrationId, IEnumerable<MigrationOperation> operations, string sourceModel, string targetModel, string @namespace, string className)
        {
            return base.Generate(migrationId, newOperations, sourceModel, targetModel, @namespace, className);
        }
       

        protected override IEnumerable<string> GetNamespaces(IEnumerable<MigrationOperation> operations)
        {
            //TODO: pridat namespacy s extension metodama pro nove databazove migracni operace
            return base.GetNamespaces(operations);
        }

    }
}
