using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions;
using System.ComponentModel.DataAnnotations.Schema;
using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;


namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public class DbMigrationOperationBuilder : IDbMigrationOperationBuilder
    {
        public EfModel OldModel { get; private set; }
        public EfModel NewModel { get; private set; }

        public DbMigrationOperationBuilder(EfModel oldModel, EfModel newModel)
        {
            this.OldModel = oldModel;
            this.NewModel = newModel;
        }

        public CreateTableOperation CreateTableOperation(EntitySet storageEntitySet)
        {
            Check.NotNull(storageEntitySet, "storageEntitySet");

            return CreateTableOperationInternal(storageEntitySet, NewModel);
        }

        private CreateTableOperation CreateTableOperationInternal(EntitySet storageEntitySet, EfModel model)
        {
            var operation = new CreateTableOperation(storageEntitySet.FullTableName());

            //add columns
            var providerManifest = model.Metadata.ProviderManifest;
            operation.Columns.AddRange(
                    storageEntitySet.ElementType.Properties.Select(p => p.ToColumnModel(providerManifest))
                );

            // add primary keys
            var addPrimaryKeyOp = new AddPrimaryKeyOperation();
            addPrimaryKeyOp.Columns.AddRange(
                    storageEntitySet.ElementType.KeyProperties.Select(k => k.Name)
                );

            operation.PrimaryKey = addPrimaryKeyOp;

            return operation;
        }

        public DropTableOperation DropTableOperation(EntitySet storageEntitySet)
        {
            Check.NotNull(storageEntitySet, "storageEntitySet");

            var inverse = CreateTableOperationInternal(storageEntitySet, OldModel);

            return new DropTableOperation(storageEntitySet.FullTableName(), inverse);
        }

        public AddColumnOperation AddColumnOperation(EntitySet storageEntitySet, EdmProperty column)
        {
            Check.NotNull(storageEntitySet, "storageEntitySet");
            Check.NotNull(column, "column");

            return AddColumnOperationInternal(storageEntitySet, column, NewModel);
        }

        private AddColumnOperation AddColumnOperationInternal(EntitySet storageEntitySet, EdmProperty column, EfModel model)
        {
            var columnModel = column.ToColumnModel(model.Metadata.ProviderManifest);

            return new AddColumnOperation(storageEntitySet.FullTableName(), columnModel);
        }

        public DropColumnOperation DropColumnOperation(EntitySet storageEntitySet, EdmProperty column)
        {
            Check.NotNull(storageEntitySet, "storageEntitySet");
            Check.NotNull(column, "columnName");

            var inverse = AddColumnOperationInternal(storageEntitySet, column, OldModel);

            return new DropColumnOperation(storageEntitySet.FullTableName(), column.Name, inverse);
        }

        public RenameTableOperation RenameTableOperation(EntitySet oldStorageEntitySet, EntitySet newStorageEntitySet)
        {
            Check.NotNull(oldStorageEntitySet, "oldStorageEntitySet");
            Check.NotNull(newStorageEntitySet, "newStorageEntitySet");

            //oldTableName is with schema, newName is NOT
            return new RenameTableOperation(oldStorageEntitySet.FullTableName(), newStorageEntitySet.Table);
        }

        public RenameColumnOperation RenameColumnOperation(EntitySet storageEntitySet, EdmProperty oldColumn, EdmProperty newColumn)
        {
            Check.NotNull(storageEntitySet, "storageEntitySet");
            Check.NotNull(oldColumn, "oldColumn");
            Check.NotNull(newColumn, "newColumn");

            return new RenameColumnOperation(storageEntitySet.FullTableName(), oldColumn.Name, newColumn.Name);
        }


        public AddForeignKeyOperation AddForeignKeyOperation(ReferentialConstraint referentialConstraint)
        {
            Check.NotNull(referentialConstraint, "referentialConstraint");

            return AddForeignKeyOperationInternal(referentialConstraint, NewModel);
        }

        private AddForeignKeyOperation AddForeignKeyOperationInternal(ReferentialConstraint referentialConstraint, EfModel model)
        {
            var operation = new AddForeignKeyOperation();
            BuildForeignKeyOperation(operation, referentialConstraint, model);

            operation.PrincipalColumns.AddRange(
                    referentialConstraint.FromProperties.Select(p => p.Name)
                );

            operation.CascadeDelete = referentialConstraint.FromRole.DeleteBehavior == OperationAction.Cascade;

            return operation;
        }

        public DropForeignKeyOperation DropForeignKeyOperation(ReferentialConstraint referentialConstraint)
        {
            Check.NotNull(referentialConstraint, "referentialConstraint");

            var inverse = AddForeignKeyOperationInternal(referentialConstraint, OldModel);
            var operation = new DropForeignKeyOperation(inverse);

            BuildForeignKeyOperation(operation, referentialConstraint, OldModel);

            return operation;
        }

        public CreateIndexOperation TryBuildCreateIndexOperation(EntitySet storageEntitySet, IEnumerable<EdmProperty> columns)
        {
            Check.NotNull(storageEntitySet, "storageEntitySet");
            Check.NotNullOrEmpty(columns, "columns");

            ConsolidatedIndex index;
            if (ForeignKeyIndexBuilder.TryBuild(storageEntitySet.FullTableName(), columns, out index))
            {
                return index.CreateCreateIndexOperation();
            }
            return null;
        }

        public DropIndexOperation TryBuildDropIndexOperation(EntitySet storageEntitySet, IEnumerable<EdmProperty> columns)
        {
            Check.NotNull(storageEntitySet, "storageEntitySet");
            Check.NotNullOrEmpty(columns, "columns");

            ConsolidatedIndex index;
            if (ForeignKeyIndexBuilder.TryBuild(storageEntitySet.FullTableName(), columns, out index))
            {
                return index.CreateDropIndexOperation();
            }
            return null;
        }


        public AddIdentityOperation TryBuildAddIdentityOperation(EntitySet storageEntitySet)
        {
            return TryBuildIdentityOperation(storageEntitySet, NewModel, () => new AddIdentityOperation());
        }

        public DropIdentityOperation TryBuildDropIdentityOperation(EntitySet storageEntitySet)
        {
            return TryBuildIdentityOperation(storageEntitySet, OldModel, () => new DropIdentityOperation());
        }

        private T TryBuildIdentityOperation<T>(EntitySet storageEntitySet, EfModel model, Func<T> operationFactory) where T : IdentityOperation
        {
            var primaryKeys = storageEntitySet.ElementType.KeyProperties;
            if (primaryKeys.Any(p => p.IsStoreGeneratedIdentity) && primaryKeys.Count <= 1)
            {
                var principalPk = primaryKeys.Single();

                var dependentColumns = model.GetAssociationSetsForEntitySet(storageEntitySet)
                    .Where(a => a.ElementType.Constraint.FromRole.GetEntityType() == storageEntitySet.ElementType)
                    .Select(a => new DependentColumn()
                    {
                        DependentTable = a.AssociationSetEnds.ElementAt(1).EntitySet.FullTableName(),
                        ForeignKeyColumn = a.ElementType.Constraint.ToProperties.Single().Name
                    }
                    );

                var operation = operationFactory();
                
                operation.PrincipalTable = storageEntitySet.FullTableName();
                operation.PrincipalColumn = principalPk.ToColumnModel(model.Metadata.ProviderManifest);
                operation.DependentColumns.AddRange(dependentColumns);

                return operation;
            }

            return null;
        }

        private void BuildForeignKeyOperation(ForeignKeyOperation operation, ReferentialConstraint referentialConstraint, EfModel model)
        {
            operation.PrincipalTable = model.Metadata.StoreEntityContainer.EntitySets
                .Single(es => es.ElementType == referentialConstraint.FromRole.GetEntityType())
                .FullTableName();

            operation.DependentTable = model.Metadata.StoreEntityContainer.EntitySets
                .Single(es => es.ElementType == referentialConstraint.ToRole.GetEntityType())
                .FullTableName();

            operation.DependentColumns.AddRange(
                    referentialConstraint.ToProperties.Select(p => p.Name)
                );
        }







    }
}
