using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.DbContext
{
    public class RemoveDbSetPropertyOperation : DbContextChangeOperation
    {
        private ClassCodeModel classModel;

        public RemoveDbSetPropertyOperation(ClassCodeModel classModel)
        {
            this.classModel = classModel;
        }

        public override void ExecuteModelChanges(IModelChangesProvider provider)
        {
            provider.ChangeDbContext.RemoveDbSetPropertyForClass(classModel);
        }

        public override ModelChangeOperation Inverse()
        {
            return new AddDbSetPropertyOperation(classModel);
        }
    }
}
