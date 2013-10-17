using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations
{
    public class RenamePropertyOperation : ModelChangeOperation
    {
        private ClassCodeModel classModel;
        private PropertyCodeModel propertyModel;
        private string newName;

        public RenamePropertyOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel, string newName)
        {
            this.classModel = classModel;
            this.propertyModel = propertyModel;
            this.newName = newName;
        }

        public override void ExecuteModelChanges(IModelChangesProvider provider)
        {
            provider.RenameProperty(classModel, propertyModel, newName);
        }

        public override ModelChangeOperation Inverse()
        {
            return new RenamePropertyOperation(classModel,
                new PropertyCodeModel()
                {
                    Name = newName,
                    IsSetterPrivate = propertyModel.IsSetterPrivate,
                    Type = propertyModel.Type,
                    Visibility = propertyModel.Visibility
                },
                propertyModel.Name);
        }
    }
}
