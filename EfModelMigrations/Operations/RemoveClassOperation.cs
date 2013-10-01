using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class RemoveClassOperation : ModelChangeOperation
    {
        private ClassCodeModel classModel;

        public RemoveClassOperation(ClassCodeModel classModel)
        {
            this.classModel = classModel;
        }
        
        public override void ExecuteModelChanges(IModelChangesProvider provider)
        {
            provider.RemoveClass(classModel);
        }

        public override ModelChangeOperation Inverse()
        {
            return new CreateClassOperation(classModel);
        }
    }
}
