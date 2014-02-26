using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public abstract class AddAssociationWithCascadeDeleteTransformation : AddAssociationTransformation
    {
        public bool WillCascadeOnDelete { get; private set; }

        public AddAssociationWithCascadeDeleteTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, bool willCascadeOnDelete)
            :base(principal, dependent)
        {
            this.WillCascadeOnDelete = willCascadeOnDelete;
        }
    }
}
