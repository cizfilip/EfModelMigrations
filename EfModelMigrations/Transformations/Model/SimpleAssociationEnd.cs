namespace EfModelMigrations.Transformations.Model
{
    public sealed class SimpleAssociationEnd
    {
        public string ClassName { get; private set; }
        public string NavigationPropertyName { get; private set; }

        public bool HasNavigationPropertyName
        {
            get
            {
                return !string.IsNullOrEmpty(NavigationPropertyName);
            }
        }

        public SimpleAssociationEnd(string className, string navigationPropertyName)
        {
            Check.NotEmpty(className, "className");

            this.ClassName = className;
            this.NavigationPropertyName = navigationPropertyName;
        }

        public SimpleAssociationEnd(string className)
            : this(className, null)
        {
        }

    }
}
