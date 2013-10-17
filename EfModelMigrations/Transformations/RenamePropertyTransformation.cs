using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RenamePropertyTransformation : ModelTransformation
    {
        public string ClassName { get; private set; }
        public string OldPropertyName { get; private set; }
        public string NewPropertyName { get; private set; }

        public RenamePropertyTransformation(string className, string oldPropertyName, string newPropertyName)
        {
            this.ClassName = className;
            this.OldPropertyName = oldPropertyName;
            this.NewPropertyName = newPropertyName;
        }

        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.GetClassCodeModel(ClassName);

            var oldPropertyModel = classModel.Properties.Single(p => p.Name.EqualsOrdinalIgnoreCase(OldPropertyName));

            yield return new RenamePropertyOperation(classModel, oldPropertyModel, NewPropertyName);
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.RenameColumnOperation(ClassName, OldPropertyName, NewPropertyName);
        }

        public override ModelTransformation Inverse()
        {
            return new RenamePropertyTransformation(ClassName, NewPropertyName, OldPropertyName);
        }
    }
}
