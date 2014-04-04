using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    //TODO: pro many to many zda se neni zpusob jak explicitne namapovat ze fk v join table budou mit indexy - tudiz spoleham na konvence....
    public class AddManyToManyAssociationTransformation : AddAssociationTransformation
    {
        public ManyToManyJoinTable JoinTable { get; private set; }

        public AddManyToManyAssociationTransformation(AssociationEnd source, AssociationEnd target, ManyToManyJoinTable joinTable)
            : base(source, target)
        {
            this.JoinTable = joinTable;

            //TODO: stringy do resourců
            if (!(source.Multipticity == RelationshipMultiplicity.Many && target.Multipticity == RelationshipMultiplicity.Many))
            {
                throw new ModelTransformationValidationException("Invalid association multiplicity for many to many association.");
            }
        }


        protected override AddAssociationMapping CreateAssociationMappingInformation(IClassModelProvider modelProvider)
        {
            return new AddAssociationMapping(Principal, Dependent)
                {
                    JoinTable = JoinTable
                };
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            //add join table
            var joinTableEntitySet = builder.NewModel.GetStoreEntitySetJoinTableForManyToMany(Principal, Dependent);
            yield return builder.CreateTableOperation(joinTableEntitySet);

            //add foreign keys and indexes
            var joinTableRelations = joinTableEntitySet.EntityContainer.AssociationSets
                .Where(a => a.AssociationSetEnds.ElementAt(1).EntitySet == joinTableEntitySet);

            foreach (var relation in joinTableRelations)
            {
                var referentialConstraint = relation.ElementType.Constraint;

                var indexOperation = builder.TryBuildCreateIndexOperation(joinTableEntitySet, referentialConstraint.ToProperties);
                if (indexOperation != null)
                {
                    yield return indexOperation;
                }
                yield return builder.AddForeignKeyOperation(referentialConstraint);
            }
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveManyToManyAssociationTransformation(Principal.ToSimpleAssociationEnd(), Dependent.ToSimpleAssociationEnd());
        }
    }
}
