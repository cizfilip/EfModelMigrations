using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class EfFluentApiCallChain : IFluentInterface
    {
        public string EntityType { get; private set; }
        public IList<EfFluetApiCall> FluentApiCalls { get; set; }

        public EfFluentApiCallChain(string entityType)
        {
            Check.NotEmpty(entityType, "entityType");

            this.EntityType = entityType;
            this.FluentApiCalls = new List<EfFluetApiCall>();
        }

        public EfFluentApiCallChain AddMethodCall(EfFluetApiCall methodCall)
        {
            Check.NotNull(methodCall, "methodCall");

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
