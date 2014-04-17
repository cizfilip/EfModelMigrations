namespace EfModelMigrations.Operations.Mapping.Model
{
    public class PropertySelectorParameter : IEfFluentApiMethodParameter
    {
        public string ClassName { get; private set; }

        public string[] PropertyNames { get; private set; }

        public PropertySelectorParameter(string className, string propertyName)
            :this(className, new[] { propertyName })
        {
        }

        public PropertySelectorParameter(string className, string[] propertyNames)
        {
            Check.NotEmpty(className, "className");
            Check.NotNull(propertyNames, "propertyNames");

            this.ClassName = className;
            this.PropertyNames = propertyNames;
        }
    }
}
