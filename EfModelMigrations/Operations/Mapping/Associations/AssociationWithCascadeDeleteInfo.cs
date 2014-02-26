using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public abstract class AssociationWithCascadeDeleteInfo : AssociationInfo
    {
        public bool WillCascadeOnDelete { get; private set; }

        public AssociationWithCascadeDeleteInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent, bool willCascadeOnDelete)
            :base(principal, dependent)
        {
            this.WillCascadeOnDelete = willCascadeOnDelete;
        }

        protected override void AddAdditionalMethodCalls(EfFluentApiCallChain callChain)
        {
            callChain.AddMethodCall(EfFluentApiMethods.WillCascadeOnDelete, new ValueParameter(WillCascadeOnDelete));
        }
    }
}
