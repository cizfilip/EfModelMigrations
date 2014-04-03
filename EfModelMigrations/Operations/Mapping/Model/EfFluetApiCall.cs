using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class EfFluetApiCall : IFluentInterface
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
            Check.NotNull(parameter, "parameter");

            Parameters.Add(parameter);

            return this;
        }

        public EfFluetApiCall AddParameters(IEnumerable<IEfFluentApiMethodParameter> parameters)
        {
            Check.NotNull(parameters, "parameters");

            foreach (var param in parameters)
            {
                Parameters.Add(param);
            }

            return this;
        }
    }
}
