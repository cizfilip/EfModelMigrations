namespace EfModelMigrations.Operations
{
    public class RenamePropertyOperation : IModelChangeOperation
    {
        public string ClassName { get; private set; }
        public string OldName { get; private set; }
        public string NewName { get; private set; }
        
        public RenamePropertyOperation(string className, string oldName, string newName)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(oldName, "oldName");
            Check.NotEmpty(oldName, "oldName");

            this.ClassName = className;
            this.OldName = oldName;
            this.NewName = newName;
        }

    }
}
