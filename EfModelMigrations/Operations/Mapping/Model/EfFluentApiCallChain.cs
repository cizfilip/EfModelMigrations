using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class EfFluentApiCallChain
    {
        public string EntityType { get; private set; }
        public IList<EfFluetApiCall> FluentApiCalls { get; private set; }

        public EfFluentApiCallChain(string entityType)
        {
            this.EntityType = entityType;
            this.FluentApiCalls = new List<EfFluetApiCall>();
        }

        public EfFluentApiCallChain AddMethodCall(EfFluetApiCall methodCall)
        {
            FluentApiCalls.Add(methodCall);

            return this;
        }

        public EfFluentApiCallChain AddMethodCall(EfFluentApiMethods method, IEfFluentApiMethodParameter parameter = null)
        {
            var methodCall = new EfFluetApiCall(method);

            if(parameter != null)
            {
                methodCall.AddParameter(parameter);
            }

            FluentApiCalls.Add(methodCall);

            return this;
        }

    }
}
