using System.Collections.Generic;
using EfModelMigrations.Extensions;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public sealed class EfFluentApiCallChain
    {
        public string EntityType { get; private set; }
        public IList<EfFluetApiCall> FluentApiCalls { get; private set; }

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

        public EfFluentApiCallChain AddCalls(IEnumerable<EfFluetApiCall> calls)
        {
            Check.NotNullOrEmpty(calls, "calls");

            FluentApiCalls.AddRange(calls);

            return this;
        }
    }
}
