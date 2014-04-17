using System;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class EnumParameter : IEfFluentApiMethodParameter
    {
        public int Value { get; private set; }
        public Type EnumType { get; private set; }

        public EnumParameter(Type enumType, int value)
        {
            Check.NotNull(enumType, "enumType");
            this.EnumType = enumType;
            this.Value = value;
        }
    }
}
