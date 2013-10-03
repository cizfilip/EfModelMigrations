using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.DbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RemoveClassTransformation : ModelTransformation
    {
        private CreateClassTransformation inverse;

        public string ClassName { get; private set; }
            

        public RemoveClassTransformation(string className)
        {
            this.ClassName = className;
        }

        public RemoveClassTransformation(string className, CreateClassTransformation inverse) : this(className)
        {
            this.inverse = inverse;
        }

        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var classModel = modelProvider.GetClassCodeModel(ClassName);

            yield return new RemoveDbSetPropertyOperation(classModel);
            foreach (var property in classModel.Properties)
            {
                yield return new RemovePropertyFromClassOperation(classModel, property);
            }
            yield return new RemoveClassOperation(classModel);
        }

        public override ModelTransformation Inverse()
        {
            return inverse;
        }

        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.DropTableOperation(ClassName);
        }
    }
}
