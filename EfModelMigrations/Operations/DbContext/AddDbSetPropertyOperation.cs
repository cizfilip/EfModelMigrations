using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.DbContext
{
    public class AddDbSetPropertyOperation : DbContextChangeOperation
    {
        private ClassCodeModel classModel;

        public AddDbSetPropertyOperation(ClassCodeModel classModel)
        {
            this.classModel = classModel;
        }

        public override void ExecuteModelChanges(IModelChangesProvider provider)
        {
            provider.ChangeDbContext.AddDbSetPropertyForClass(classModel);
        }

        public override ModelChangeOperation Inverse()
        {
            return new RemoveDbSetPropertyOperation(classModel);
        }
    }
}
