using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class AddPropertyToClassOperation : ModelChangeOperation
    {
        private ClassCodeModel classModel;
        private PropertyCodeModel propertyModel;

        public AddPropertyToClassOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel)
        {
            this.classModel = classModel;
            this.propertyModel = propertyModel;
        }


        public override void ExecuteModelChanges(IModelChangesProvider provider)
        {
            provider.AddPropertyToClass(classModel, propertyModel);
        }

        public override ModelChangeOperation Inverse()
        {
            return new RemovePropertyFromClassOperation(classModel, propertyModel);
        }
    }
}
