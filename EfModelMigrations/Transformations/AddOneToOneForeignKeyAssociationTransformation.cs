using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Transformations
{
    public class AddOneToOneForeignKeyAssociationTransformation : AddAssociationWithCascadeDeleteTransformation
    {   
        public string[] ForeignKeyColumnNames { get; private set; }

        public AddOneToOneForeignKeyAssociationTransformation(AssociationEnd principal, AssociationEnd dependent, string[] foreignKeyColumnNames, bool? willCascadeOnDelete = null)
            :base(principal, dependent, willCascadeOnDelete)
        {
            this.ForeignKeyColumnNames = foreignKeyColumnNames;

            //TODO: stringy do resourců
            if (principal.Multipticity == RelationshipMultiplicity.Many && dependent.Multipticity == RelationshipMultiplicity.Many)
            {
                throw new ModelTransformationValidationException("Invalid association multiplicity for one to one foreign key association.");
            }
        }

        protected override AddAssociationMapping CreateMappingInformation()
        {
            return new AddAssociationMapping(Principal, Dependent)
            {
                ForeignKeyColumnNames = ForeignKeyColumnNames,
                WillCascadeOnDelete = WillCascadeOnDelete
            };
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            bool isDependentRequired = Dependent.Multipticity == RelationshipMultiplicity.One ? true : false;

            return builder.OneToOneForeignKeyRelationOperations(Principal.ClassName, Dependent.ClassName, isDependentRequired, ForeignKeyColumnNames, WillCascadeOnDelete);
        }

        public override ModelTransformation Inverse()
        {
            return null;
        }
    }
}
