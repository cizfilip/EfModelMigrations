using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class OneToManyWithForeignKeyColumnsAssociationInfo : OneToManyAssociationInfo
    {
        public string[] ForeignKeyColumns { get; private set; }


        public OneToManyWithForeignKeyColumnsAssociationInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent, bool isDependentRequired, string[] foreignKeyColumns, bool willCascadeOnDelete)
            : base(principal, dependent, isDependentRequired, willCascadeOnDelete)
        {
            this.ForeignKeyColumns = foreignKeyColumns;
        }

        

        protected override void AddAdditionalMethodCalls(EfFluentApiCallChain callChain)
        {
            callChain.AddMethodCall(EfFluentApiMethods.Map, new MapMethodParameter().MapKey(ForeignKeyColumns));

            base.AddAdditionalMethodCalls(callChain);
        }
    }
}
