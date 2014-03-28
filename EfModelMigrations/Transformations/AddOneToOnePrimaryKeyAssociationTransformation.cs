using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class AddOneToOnePrimaryKeyAssociationTransformation : AddAssociationWithCascadeDeleteTransformation
    {
        public AddOneToOnePrimaryKeyAssociationTransformation(AssociationMemberInfo principal, AssociationMemberInfo dependent, bool? willCascadeOnDelete = null)
            :base(principal, 
                dependent,  
                willCascadeOnDelete)
        {
            //TODO: stringy do resourců
            if ((principal.Multipticity == RelationshipMultiplicity.Many && dependent.Multipticity == RelationshipMultiplicity.Many)
                || (principal.Multipticity == RelationshipMultiplicity.ZeroOrOne && dependent.Multipticity == RelationshipMultiplicity.ZeroOrOne))
            {
                throw new ModelTransformationValidationException("Invalid association multiplicity for one to one primary key association.");
            }
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            return builder.OneToOnePrimaryKeyRelationOperations(Principal.ClassName, Dependent.ClassName, WillCascadeOnDelete);
        }

        protected override AddAssociationMapping CreateMappingInformation()
        {
            return new AddAssociationMapping(Principal, Dependent)
            {
                WillCascadeOnDelete = WillCascadeOnDelete
            };
        }

        public override ModelTransformation Inverse()
        {
            return null;
        }

       
    }

    


}
