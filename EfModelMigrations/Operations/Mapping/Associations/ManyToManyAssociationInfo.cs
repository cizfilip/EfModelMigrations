using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class ManyToManyAssociationInfo : AssociationInfo
    {
        public ManyToManyJoinTable JoinTable { get; private set; }
        
        public ManyToManyAssociationInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent, ManyToManyJoinTable joinTable)
            : base(principal, dependent)
        {
            this.JoinTable = joinTable;
        }

        protected override Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromPrincipal()
        {
            return GetFluentApiMethods();
        }

        protected override Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromDependent()
        {
            return GetFluentApiMethods();
        }

        protected override void AddAdditionalMethodCalls(EfFluentApiCallChain callChain)
        {
            string[] leftKeys;
            string[] rightKeys;

            if(Principal.NavigationProperty != null)
            {
                leftKeys = JoinTable.PrincipalForeignKeyColumns;
                rightKeys = JoinTable.DependentForeignKeyColumns;
            }
            else
            {
                leftKeys = JoinTable.DependentForeignKeyColumns;
                rightKeys = JoinTable.PrincipalForeignKeyColumns;
            }

            callChain.AddMethodCall(EfFluentApiMethods.Map, 
                new MapMethodParameter()
                    .ToTable(JoinTable.TableName)
                    .MapLeftKey(leftKeys)
                    .MapRightKey(rightKeys)
                );
        }


        private Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethods()
        {
            return Tuple.Create(EfFluentApiMethods.HasMany, EfFluentApiMethods.WithMany);
        }
    }
}
