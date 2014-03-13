using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class AddOneToOnePrimaryKeyAssociationTransformation : AddOneToOneAssociationTransformation
    {
        public AddOneToOnePrimaryKeyAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, bool bothEndsRequired, bool willCascadeOnDelete)
            :base(principal, 
                dependent, 
                bothEndsRequired ? OneToOneAssociationType.BothEndsRequired : OneToOneAssociationType.DependentRequired, 
                willCascadeOnDelete)
        {
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return builder.OneToOnePrimaryKeyRelationOperations(Principal.ClassName, Dependent.ClassName, WillCascadeOnDelete);
        }

        protected override AssociationInfo CreateMappingInformation()
        {
            return new OneToOneAssociationInfo(Principal, Dependent, Type, WillCascadeOnDelete);
        }

        public override ModelTransformation Inverse()
        {
            return null;
        }

       
    }

    


}
