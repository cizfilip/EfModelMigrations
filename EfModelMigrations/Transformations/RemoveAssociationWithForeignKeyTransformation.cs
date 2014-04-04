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
    public class RemoveAssociationWithForeignKeyTransformation : RemoveAssociationTransformation
    {
        public RemoveAssociationWithForeignKeyTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, ModelTransformation inverse)
            : base(principal, dependent, inverse)
        {
        }

        public RemoveAssociationWithForeignKeyTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent)
            : this(principal, dependent, null)
        {
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            var referentialConstraint = builder.OldModel.GetStorageAssociationTypeForAssociation(Principal, Dependent)
                .Constraint;
            var dependentStoreEntitySet = builder.OldModel.GetStoreEntitySetForClass(Dependent.ClassName);
            var foreignKeyColumns = referentialConstraint.ToProperties;

            //remove foreign key
            yield return builder.DropForeignKeyOperation(referentialConstraint);

            //remove index on Fk if exists on ef model
            var indexOperation = builder.TryBuildDropIndexOperation(dependentStoreEntitySet, foreignKeyColumns);
            if (indexOperation != null)
            {
                yield return indexOperation;
            }

            //remove fk columns
            foreach (var foreignKey in foreignKeyColumns)
            {
                yield return builder.DropColumnOperation(dependentStoreEntitySet, foreignKey);
            }
        }
    }
}
