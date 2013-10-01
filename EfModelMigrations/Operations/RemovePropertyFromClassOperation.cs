using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class RemovePropertyFromClassOperation : ModelChangeOperation
    {
        private ClassCodeModel classModel;
        private PropertyCodeModel propertyModel;

        public RemovePropertyFromClassOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel)
        {
            this.classModel = classModel;
            this.propertyModel = propertyModel;
        }

        public override void ExecuteModelChanges(IModelChangesProvider provider)
        {
            provider.RemovePropertyFromClass(classModel, propertyModel);
        }

        public override ModelChangeOperation Inverse()
        {
            return new AddPropertyToClassOperation(classModel, propertyModel);
        }
    }
}
