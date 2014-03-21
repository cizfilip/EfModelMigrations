using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Operations.Mapping.Model;
using System.Diagnostics;

namespace EfModelMigrations.UnitTests
{
    public class CSharpRegexMappingGeneratorTests
    {
        [Fact]
        public void AssociationTest()
        {
            string person = "Person";
            string address = "Address";

            var gen = new CSharpRegexMappingGenerator();
            var fluentCall = new EfFluentApiCallChain(person);
            fluentCall.AddMethodCall(EfFluentApiMethods.HasMany, new PropertySelectorParameter(address, person))
                .AddMethodCall(EfFluentApiMethods.WithMany);

            var result = gen.GenerateFluentApiCall(fluentCall);

            Debug.WriteLine(result.Content);
        }
    }
}
