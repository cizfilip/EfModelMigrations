using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class OneToOneAssociationInfo : AssociationWithCascadeDeleteInfo
    {
        public OneToOneAssociationType Type { get; private set; }

        public OneToOneAssociationInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent, OneToOneAssociationType type, bool willCascadeOnDelete)
            : base(principal, dependent, willCascadeOnDelete)
        {
            this.Type = type;
        }

        protected override Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromPrincipal()
        {
            switch (Type)
            {
                case OneToOneAssociationType.DependentRequired:
                    return Tuple.Create(EfFluentApiMethods.HasOptional, EfFluentApiMethods.WithRequired);
                case OneToOneAssociationType.BothEndsRequired:
                    return Tuple.Create(EfFluentApiMethods.HasRequired, EfFluentApiMethods.WithRequiredPrincipal);
                case OneToOneAssociationType.BothEndsOptional:
                    return Tuple.Create(EfFluentApiMethods.HasOptional, EfFluentApiMethods.WithOptionalPrincipal);
                default:
                    throw new InvalidOperationException("Invalid OneToOneAssociationType."); //TODO: string do resourcu
            }
        }

        protected override Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromDependent()
        {
            switch (Type)
            {
                case OneToOneAssociationType.DependentRequired:
                    return Tuple.Create(EfFluentApiMethods.HasRequired, EfFluentApiMethods.WithOptional);
                case OneToOneAssociationType.BothEndsRequired:
                    return Tuple.Create(EfFluentApiMethods.HasRequired, EfFluentApiMethods.WithRequiredDependent);
                case OneToOneAssociationType.BothEndsOptional:
                    return Tuple.Create(EfFluentApiMethods.HasOptional, EfFluentApiMethods.WithOptionalDependent);
                default:
                    throw new InvalidOperationException("Invalid OneToOneAssociationType."); //TODO: string do resourcu
            }
        }

        protected override void AddAdditionalMethodCalls(EfFluentApiCallChain callChain)
        {
            base.AddAdditionalMethodCalls(callChain);
        }
    }
}
