using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations.Model;
using System.Linq;
using EfModelMigrations.Extensions;
using System.Data.Entity.Migrations.Model;
using System.Collections.Generic;
using EfModelMigrations.Infrastructure.EntityFramework;
using System;

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
            var referentialConstraint = builder.NewModel.GetStoreAssociationTypeForAssociation(Principal, Dependent)
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

        public static string[] GetUniquifiedDefaultForeignKeyColumnNames(AssociationEnd principal, AssociationEnd dependent, ClassCodeModel principalCodeModel, ClassCodeModel dependentCodeModel)
        {
            Check.NotNull(principal, "principal");
            Check.NotNull(dependent, "dependent");
            Check.NotNull(principalCodeModel, "principalCodeModel");
            Check.NotNull(dependentCodeModel, "dependentCodeModel");

            var dependentColumnNames = dependentCodeModel.StoreEntityType.Properties.Select(p => p.Name);
            return GetDefaultForeignKeyColumnNamesInternal(principal, dependent, principalCodeModel, c => dependentColumnNames.Uniquify(c));
        }

        public static string[] GetDefaultForeignKeyColumnNames(AssociationEnd principal, AssociationEnd dependent, ClassCodeModel principalCodeModel)
        {
            Check.NotNull(principal, "principal");
            Check.NotNull(dependent, "dependent");
            Check.NotNull(principalCodeModel, "principalCodeModel");

            return GetDefaultForeignKeyColumnNamesInternal(principal, dependent, principalCodeModel);
        }

        private static string[] GetDefaultForeignKeyColumnNamesInternal(AssociationEnd principal, AssociationEnd dependent, ClassCodeModel principalCodeModel, Func<string, string> columnNameModificator = null)
        {
            string prefix = dependent.HasNavigationProperty ? dependent.NavigationProperty.Name : principal.ClassName;

            return principalCodeModel.PrimaryKeys.Select(
                p => string.Concat(prefix, "_", columnNameModificator != null ? columnNameModificator(p.Column.ColumnName) : p.Column.ColumnName)
                ).ToArray();
        }
    }
}
