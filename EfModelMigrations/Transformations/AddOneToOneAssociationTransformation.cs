using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public abstract class AddOneToOneAssociationTransformation : AddAssociationWithCascadeDeleteTransformation
    {
        public OneToOneAssociationType Type { get; private set; }

        public AddOneToOneAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, OneToOneAssociationType type, bool willCascadeOnDelete)
            :base(principal, dependent, willCascadeOnDelete)
        {
            this.Type = type;
        }
        
        protected override AssociationInfo CreateMappingInformation()
        {
            return new OneToOneAssociationInfo(Principal, Dependent, Type, WillCascadeOnDelete);
        }

    }
}
