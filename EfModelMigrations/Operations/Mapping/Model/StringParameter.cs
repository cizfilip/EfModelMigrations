namespace EfModelMigrations.Operations.Mapping.Model
{   
    public class StringParameter : IEfFluentApiMethodParameter
    {
        public string Value { get; private set; }

        public StringParameter(string value)
        {
            Check.NotEmpty(value, "value");

            this.Value = value;
        }
    }
}
