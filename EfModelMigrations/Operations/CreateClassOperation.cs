using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;

namespace EfModelMigrations.Operations
{
    public class CreateClassOperation : ModelChangeOperation
    {
        private ClassCodeModel classModel;

        public CreateClassOperation(ClassCodeModel classModel)
        {
            this.classModel = classModel;
        }

        public override void ExecuteModelChanges(IModelChangesProvider provider)
        {
            provider.CreateEmptyClass(classModel);
        }

        public override ModelChangeOperation Inverse()
        {
            return new RemoveClassOperation(classModel);
        }
    }
}
