using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public abstract class OneToManyAssociationInfo : AssociationWithCascadeDeleteInfo
    {
        public bool IsDependentRequired { get; private set; }

        public OneToManyAssociationInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent, bool isDependentRequired, bool willCascadeOnDelete)
            : base(principal, dependent, willCascadeOnDelete)
        {
            this.IsDependentRequired = isDependentRequired;
        }

        protected override Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromPrincipal()
        {
            if (IsDependentRequired)
            {
                return Tuple.Create(EfFluentApiMethods.HasMany, EfFluentApiMethods.WithRequired);
            }
            else
            {
                return Tuple.Create(EfFluentApiMethods.HasMany, EfFluentApiMethods.WithOptional);
            }
        }

        protected override Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromDependent()
        {
            if (IsDependentRequired)
            {
                return Tuple.Create(EfFluentApiMethods.HasRequired, EfFluentApiMethods.WithMany);
            }
            else
            {
                return Tuple.Create(EfFluentApiMethods.HasOptional, EfFluentApiMethods.WithMany);
            }
        }
    }
}
