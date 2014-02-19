using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class PropertySelectorParameter : IEfFluentApiMethodParameter
    {
        public string ClassName { get; private set; }

        public string PropertyName { get; private set; }

        public PropertySelectorParameter(string className, string propertyName)
        {
            this.ClassName = className;
            this.PropertyName = propertyName;
        }
    }
}
