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
    public class RemoveOneToOnePrimaryKeyAssociationTransformation : RemoveAssociationTransformation
    {
        public bool AddIdentityOnDependentPk { get; private set; }

        public RemoveOneToOnePrimaryKeyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, bool addIdentityOnDependentPk, AddOneToOnePrimaryKeyAssociationTransformation inverse)
            : base(principal, dependent, inverse)
        {
            this.AddIdentityOnDependentPk = addIdentityOnDependentPk;
        }

        public RemoveOneToOnePrimaryKeyAssociationTransformation(SimpleAssociationEnd principal, SimpleAssociationEnd dependent, bool addIdentityOnDependentPk)
            : this(principal, dependent, addIdentityOnDependentPk, null)
        {
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            var dependentStoreEntitySet = builder.OldModel.GetStoreEntitySetForClass(Dependent.ClassName);

            var referentialConstraint = builder.OldModel.GetStoreAssociationTypeForAssociation(Principal, Dependent)
                .Constraint;
            var foreignKeyColumns = referentialConstraint.ToProperties;

            //drop foreign key
            yield return builder.DropForeignKeyOperation(referentialConstraint);

            //drop index on Fk if exists on ef model
            var indexOperation = builder.TryBuildDropIndexOperation(dependentStoreEntitySet, foreignKeyColumns);
            if (indexOperation != null)
            {
                yield return indexOperation;
            }

            //add identity if required
            if(AddIdentityOnDependentPk)
            {
                var addIdentityOperation = builder.TryBuildAddIdentityOperation(builder.NewModel.GetStoreEntitySetForClass(Dependent.ClassName));
                if (addIdentityOperation != null)
                {
                    yield return addIdentityOperation;
                }    
            }
        }
    }
}
