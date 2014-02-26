using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class OneToManyWithForeignKeyPropertiesAssociationInfo : OneToManyAssociationInfo
    {
        public string[] ForeignKeyProperties { get; private set; }


        public OneToManyWithForeignKeyPropertiesAssociationInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent, bool isDependentRequired, string[] foreignKeyProperties, bool willCascadeOnDelete)
            : base(principal, dependent, isDependentRequired, willCascadeOnDelete)
        {
            this.ForeignKeyProperties = foreignKeyProperties;
        }


        protected override void AddAdditionalMethodCalls(EfFluentApiCallChain callChain)
        {
            callChain.AddMethodCall(EfFluentApiMethods.HasForeignKey, new PropertySelectorParameter(Dependent.ClassName, ForeignKeyProperties));

            base.AddAdditionalMethodCalls(callChain);
        }
    }
}
