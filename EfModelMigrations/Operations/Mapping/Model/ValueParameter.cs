namespace EfModelMigrations.Operations.Mapping.Model
{
    public class ValueParameter : IEfFluentApiMethodParameter
    {
        public object Value { get; private set; }

        public ValueParameter(object value)
        {
            Check.NotNull(value, "value");

            this.Value = value;
        }
    }
}
