using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public enum EfFluentApiMethods
    {
        HasRequired,
        WithRequired,
        HasOptional,
        WithOptional,
        WithRequiredPrincipal,
        WithRequiredDependent,
        WithOptionalPrincipal,
        WithOptionalDependent,

        WillCascadeOnDelete,
    }
}
