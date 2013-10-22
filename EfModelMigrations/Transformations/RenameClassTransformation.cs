using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RenameClassTransformation : ModelTransformation
    {
        public string OldClassName { get; private set; }
        public string NewClassName { get; private set; }

        public RenameClassTransformation(string oldClassName, string newClassName)
        {
            this.OldClassName = oldClassName;
            this.NewClassName = newClassName;
        }

        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations(IClassModelProvider modelProvider)
        {
            var oldClassModel = modelProvider.GetClassCodeModel(OldClassName);

            yield return new RenameClassOperation(oldClassModel, NewClassName);
        }

        //TODO: V DB by bylo treba prejmenovat i vsechny reference - napr. jmena cizich klicu atd... ??
        public override MigrationOperation GetDbMigrationOperation(IDbMigrationOperationBuilder builder)
        {
            return builder.RenameTableOperation(OldClassName, NewClassName);
        }

        public override ModelTransformation Inverse()
        {
            return new RenameClassTransformation(NewClassName, OldClassName);
        }
    }
}
