using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RemoveManyToManyAssociationTransformation : RemoveAssociationTransformation
    {
        public RemoveManyToManyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, ModelTransformation inverse)
            : base(principal, dependent, inverse)
        {
        }

        public RemoveManyToManyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent)
            : this(principal, dependent, null)
        {
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            var joinTableEntitySet = builder.OldModel.GetStoreEntitySetJoinTableForManyToMany(Principal, Dependent);
            
            //drop foreign keys and indexes
            var joinTableRelations = joinTableEntitySet.EntityContainer.AssociationSets
                .Where(a => a.AssociationSetEnds.ElementAt(1).EntitySet == joinTableEntitySet);

            foreach (var relation in joinTableRelations)
            {
                var referentialConstraint = relation.ElementType.Constraint;

                yield return builder.DropForeignKeyOperation(referentialConstraint);

                var indexOperation = builder.TryBuildDropIndexOperation(joinTableEntitySet, referentialConstraint.ToProperties);
                if (indexOperation != null)
                {
                    yield return indexOperation;
                }
            }

            //drop join table
            yield return builder.DropTableOperation(joinTableEntitySet);
        }
    }
}
