using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations.Model;
using System.Linq;
using EfModelMigrations.Extensions;
using System.Data.Entity.Migrations.Model;
using System.Collections.Generic;
using EfModelMigrations.Infrastructure.EntityFramework;

namespace EfModelMigrations.Transformations
{
    public abstract class AddAssociationWithForeignKeyTransformation : AddAssociationWithCascadeDeleteTransformation
    {

        public AddAssociationWithForeignKeyTransformation(AssociationEnd principal, AssociationEnd dependent, bool? willCascadeOnDelete = null)
            : base(principal, dependent, willCascadeOnDelete)
        {
        }

        public override IEnumerable<MigrationOperation> GetDbMigrationOperations(IDbMigrationOperationBuilder builder)
        {
            var referentialConstraint = builder.NewModel.GetStorageAssociationTypeForAssociation(Principal, Dependent)
                .Constraint;
            var dependentStoreEntitySet = builder.NewModel.GetStoreEntitySetForClass(Dependent.ClassName);
            var foreignKeyColumns = referentialConstraint.ToProperties;

            //Add fk columns
            foreach (var foreignKey in foreignKeyColumns)
            {
                yield return builder.AddColumnOperation(dependentStoreEntitySet, foreignKey);
            }

            //add index on Fk if exists on ef model
            var indexOperation = builder.TryBuildCreateIndexOperation(dependentStoreEntitySet, foreignKeyColumns);
            if (indexOperation != null)
            {
                yield return indexOperation;
            }

            //add foreign key
            yield return builder.AddForeignKeyOperation(referentialConstraint);
        }

        protected string[] GetDefaultForeignKeyColumnNames(ClassCodeModel principalCodeModel, ClassCodeModel dependentCodeModel)
        {
            string prefix = Dependent.HasNavigationProperty ? Dependent.NavigationProperty.Name : Principal.ClassName;

            var dependentColumnNames = dependentCodeModel.StoreEntityType.Properties.Select(p => p.Name);

            return principalCodeModel.PrimaryKeys.Select(
                    p => string.Concat(prefix, "_", dependentColumnNames.Uniquify(p.Column.ColumnName))
                ).ToArray();
        }
    }
}
