using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class EfFluetApiCall
    {
        public EfFluentApiMethods Method { get; private set; }

        public IList<IEfFluentApiMethodParameter> Parameters { get; private set; }

        public EfFluetApiCall(EfFluentApiMethods method)
        {
            this.Method = method;
            this.Parameters = new List<IEfFluentApiMethodParameter>();
        }

        public EfFluetApiCall AddParameter(IEfFluentApiMethodParameter parameter)
        {
            Parameters.Add(parameter);

            return this;
        }
    }
}
