using EfModelMigrations.Infrastructure.CodeModel;

namespace EfModelMigrations.Operations
{
    public class AddPropertyToClassOperation : IModelChangeOperation
    {
        public string ClassName { get; private set; }
        public PropertyCodeModel Model { get; private set; }

        public AddPropertyToClassOperation(string className, PropertyCodeModel model)
        {
            Check.NotEmpty(className, "className");
            Check.NotNull(model, "model");

            this.ClassName = className;
            this.Model = model;
        }
        
    }
}
