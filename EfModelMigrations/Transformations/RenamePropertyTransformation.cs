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
        public string OldName { get; private set; }
        public string NewName { get; private set; }

        public RenamePropertyTransformation(string className, string oldName, string newName)
        {
            this.ClassName = className;
            this.OldName = oldName;
            this.NewName = newName;
        }

        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            yield return new RenamePropertyOperation(ClassName, OldName, NewName);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.RenameColumnOperation(ClassName, OldName, NewName);
        }

        public override ModelTransformation Inverse()
        {
            return new RenamePropertyTransformation(ClassName, NewName, OldName);
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
