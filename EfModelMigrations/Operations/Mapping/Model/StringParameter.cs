using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{   
    public class StringParameter : IEfFluentApiMethodParameter
    {
        public string Value { get; private set; }

        public StringParameter(string value)
        {
            this.Value = value;
        }
    }
}
