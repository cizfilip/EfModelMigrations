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
        HasMany,
        WithMany,



        WillCascadeOnDelete,


        //Map method
        Map,
        MapKey,
        //Map for many to many
        MapLeftKey,
        MapRightKey,
        ToTable,

        HasForeignKey,
    }
}
