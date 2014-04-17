using EfModelMigrations.Infrastructure.CodeModel;

namespace EfModelMigrations.Operations
{
    public class CreateEmptyClassOperation : IModelChangeOperation
    {
        public string Name { get; private set; }
        public CodeModelVisibility? Visibility { get; private set; }


        public CreateEmptyClassOperation(string name, CodeModelVisibility? visibility = null)
        {
            Check.NotEmpty(name, "name");

            this.Name = name;
            this.Visibility = visibility;
        }
    }
}
