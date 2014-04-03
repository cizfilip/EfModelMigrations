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
    public class AddPropertyMapping : IAddMappingInformation
    {
        public ScalarPropertyCodeModel Property { get; private set; }

        public IndexAttribute Index { get; set; }

        public AddPropertyMapping(ScalarPropertyCodeModel property)
        {
            Check.NotNull(property, "property");

            this.Property = property;
        }

        public EfFluentApiCallChain BuildEfFluentApiCallChain()
        {
            throw new NotImplementedException();
        }
    }
}
