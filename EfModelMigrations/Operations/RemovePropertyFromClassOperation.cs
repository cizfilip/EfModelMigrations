namespace EfModelMigrations.Operations
{
    public class RemovePropertyFromClassOperation : IModelChangeOperation
    {
        public string ClassName { get; private set; }
        public string Name { get; private set; }
        
        public RemovePropertyFromClassOperation(string className, string name)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(name, "name");

            this.ClassName = className;
            this.Name = name;
        }

    }
}
