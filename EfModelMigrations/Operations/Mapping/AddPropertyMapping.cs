using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Operations.Mapping.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    //TODO: dodelat mapping informaci pro property
    //TODO: pokud nebude treba nic mapovat vracet prazdnej EfFluentCallChain a nejak resit ve VsModelChangesExecutor ??
    public class AddPropertyMapping : IAddMappingInformation
    {
        public string ClassName { get; private set; }
        public ScalarPropertyCodeModel Property { get; private set; }

        public IndexAttribute Index { get; set; }

        public AddPropertyMapping(string className, ScalarPropertyCodeModel property)
        {
            Check.NotEmpty(className, "className");
            Check.NotNull(property, "property");

            this.ClassName = className;
            this.Property = property;
        }

        public EfFluentApiCallChain BuildEfFluentApiCallChain()
        {
            throw new NotImplementedException();
        }
    }
}
