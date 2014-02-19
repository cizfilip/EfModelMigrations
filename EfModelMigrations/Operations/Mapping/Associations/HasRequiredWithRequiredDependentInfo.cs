using EfModelMigrations.Operations.Mapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class HasRequiredWithRequiredDependentInfo : AssociationMappingInformation
    {
        public HasRequiredWithRequiredDependentInfo(string fromEntityName, string fromNavigationProperty, bool willCascadeOnDelete)
            : base(fromEntityName, fromNavigationProperty, null, null, willCascadeOnDelete)
        {
        }

        public HasRequiredWithRequiredDependentInfo(string fromEntityName, string fromNavigationProperty, string toEntityName, string toNavigationProperty, bool willCascadeOnDelete)
            : base(fromEntityName, fromNavigationProperty, toEntityName, toNavigationProperty, willCascadeOnDelete)
        {
        }


        public override EfFluentApiCallChain BuildEfFluentApiCallChain()
        {
            return BuildAssociationEfFluentApiCallChain(EfFluentApiMethods.HasRequired, EfFluentApiMethods.WithRequiredDependent);
        }
    }
}
