using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class AddOneToOnePrimaryKeyAssociationTransformation : AddAssociationWithCascadeDeleteTransformation
    {
        public AddOneToOnePrimaryKeyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, bool? willCascadeOnDelete = null)
            :base(principal, dependent, willCascadeOnDelete)
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
            var dependentStoreEntitySet = builder.NewModel.GetStoreEntitySetForClass(Dependent.ClassName);

            //drop identity if required
            var dropIdentityOperation = builder.TryBuildDropIdentityOperation(dependentStoreEntitySet);
            if(dropIdentityOperation != null)
            {
                yield return dropIdentityOperation;
            }

            var referentialConstraint = builder.NewModel.GetStorageAssociationTypeForAssociation(Principal, Dependent)
                .Constraint;
            var foreignKeyColumns = referentialConstraint.ToProperties;

            //add index on Fk if exists on ef model
            var indexOperation = builder.TryBuildCreateIndexOperation(dependentStoreEntitySet, foreignKeyColumns);
            if (indexOperation != null)
            {
                yield return indexOperation;
            }

            //add foreign key
            yield return builder.AddForeignKeyOperation(referentialConstraint);
        }

        protected override AddAssociationMapping CreateAssociationMappingInformation(IClassModelProvider modelProvider)
        {
            return new AddAssociationMapping(Principal, Dependent)
            {
                WillCascadeOnDelete = WillCascadeOnDelete
            };
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveOneToOnePrimaryKeyAssociationTransformation(Principal.ToSimpleAssociationEnd(), Dependent.ToSimpleAssociationEnd(), true);
        }

       
    }

    


}
