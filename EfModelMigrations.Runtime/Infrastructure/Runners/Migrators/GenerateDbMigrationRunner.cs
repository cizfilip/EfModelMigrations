using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Runtime.Infrastructure.Migrations;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.Migrators
{
    [Serializable]
    internal class GenerateDbMigrationRunner : MigratorBaseRunner
    {
        public bool IsRevert { get; set; }

        public override void Run()
        {
            var operationBuilder = new DbMigrationOperationBuilder(DbContext);

            IEnumerable<MigrationOperation> dbMigrationOperations = GetModelTransformations(IsRevert)
                .Select(t => t.GetDbMigrationOperation(operationBuilder));
                        

            DbMigrationsConfiguration configuration = CreateInstance<DbMigrationsConfiguration>(Configuration.EfMigrationsConfigurationType);

            configuration.CodeGenerator = new ExtendedCSharpMigrationCodeGenerator(dbMigrationOperations);

            MigrationScaffolder scaffolder = new MigrationScaffolder(configuration);

            string dbMigrationName = ModelMigration.Name;
            var scaffoldedMigration = scaffolder.Scaffold(dbMigrationName, ignoreChanges: true);

            Return(scaffoldedMigration);
        }
    }
}
