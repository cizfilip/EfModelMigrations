using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class OneToOneWithForeignKeysAssociationInfo : OneToOneAssociationInfo
    {
        public string[] ForeignKeyColumnNames { get; private set; }

        public OneToOneWithForeignKeysAssociationInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent, OneToOneAssociationType type, string[] foreignKeyColumnNames, bool willCascadeOnDelete)
            : base(principal, dependent, type, willCascadeOnDelete)
        {
            this.ForeignKeyColumnNames = foreignKeyColumnNames;
        }

        protected override void AddAdditionalMethodCalls(EfFluentApiCallChain callChain)
        {
            callChain.AddMethodCall(EfFluentApiMethods.Map, new MapMethodParameter().MapKey(ForeignKeyColumnNames));

            base.AddAdditionalMethodCalls(callChain);
        }

    }
}
