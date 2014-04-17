namespace EfModelMigrations.Operations
{
    public class RemoveClassOperation : IModelChangeOperation
    {
        public string Name { get; private set; }

        public RemoveClassOperation(string name)
        {
            Check.NotEmpty(name, "name");

            this.Name = name;
        }
        
    }
}
