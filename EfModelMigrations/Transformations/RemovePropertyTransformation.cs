using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class RemovePropertyTransformation : TransformationWithInverse
    {
        public string ClassName { get; private set; }
        public string Name { get; private set; }

        public RemovePropertyTransformation(string className, string name, ModelTransformation inverse)
            : base(inverse)
        {
            this.ClassName = className;
            this.Name = name;
        }

        public RemovePropertyTransformation(string className, string name)
            : this(className, name, null)
        {
        }

       
        public override IEnumerable<IModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            //TODO: Validovat zda property vůbec existuje
            //TODO: stejnej kod je i v RemovePropertiesCommand

            yield return new RemovePropertyFromClassOperation(ClassName, Name);
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            yield return builder.DropColumnOperation(ClassName, Name);
        }

        public override bool IsDestructiveChange
        {
            get { return true; }
        }
    }
}
