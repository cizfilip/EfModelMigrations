using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Common;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public class DbMigrationOperationBuilder : IDbMigrationOperationBuilder
    {
        private MetadataWorkspace metadata;

        public DbMigrationOperationBuilder(DbContext context)
        {
            this.metadata = (context as IObjectContextAdapter).ObjectContext.MetadataWorkspace;
        }

        #region IDbMigrationOperationBuilder implementation
        

        public CreateTableOperation CreateTableOperation(ClassCodeModel classModel)
        {
            //get table name
            string tableName = metadata.GetTableNameForClass(classModel);
            CreateTableOperation createTableOperation = new CreateTableOperation(tableName);

            //map properties to colums 
            //TODO: mapuji se vsechny properties ne jenom ty v classModel - zde asi ok
            IEnumerable<ColumnModel> columns = metadata.GetTableColumnsForClass(classModel).Select(prop => CreateColumnModel(prop));
            foreach (var column in columns)
            {
                createTableOperation.Columns.Add(column);
            }

            //add primary keys
            var addPrimaryKeyOperation = new AddPrimaryKeyOperation();
            var keyMembers = metadata.GetTableKeyColumnsForClass(classModel);
            foreach (var key in keyMembers)
            {
                addPrimaryKeyOperation.Columns.Add(key.Name);
            }
            createTableOperation.PrimaryKey = addPrimaryKeyOperation;

            return createTableOperation;
        }

        public DropTableOperation DropTableOperation(ClassCodeModel classModel)
        {
            string tableName = metadata.GetTableNameForClass(classModel);
            //TODO: predavat i CreateTableOperation??
            var dropTableOperation = new DropTableOperation(tableName);
            return dropTableOperation;
        }

        public AddColumnOperation AddColumnOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel)
        {
            string tableName = metadata.GetTableNameForClass(classModel);
            
            //TODO: spoleha na to ze property se jmenuje stejne jako column - fail
            var columnModel = metadata.GetTableColumnsForClass(classModel).Single(c => string.Equals(c.Name, propertyModel.Name, StringComparison.OrdinalIgnoreCase));

            var addColumnOperation = new AddColumnOperation(tableName, CreateColumnModel(columnModel));

            return addColumnOperation;
        }

        public DropColumnOperation DropColumnOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel)
        {
            string tableName = metadata.GetTableNameForClass(classModel);

            //TODO: predavat i CreateColumnOperation - ne ef nebude mit potrebu treba delat inverzi
            //TODO: spravne pretransformovat i property name
            var dropColumnOperation = new DropColumnOperation(tableName, propertyModel.Name);

            return dropColumnOperation;
        }


        //TODO: Rename operace jsou dost humus - jelikoz po aplikaci zmen na tridach se znovu vytvori ModelMigrace a Transformace ale uz se nemuze najit class model protoze uz je prejmenovano
        public RenameTableOperation RenameTableOperation(ClassCodeModel classModel, string newTableName)
        {
            //string oldTableName = metadata.GetTableNameForClass(classModel);

            ////oldTableName is with schema, newName is NOT
            //var renameTableOperation = new RenameTableOperation(oldTableName, newTableName);

            //return renameTableOperation;

            throw new NotImplementedException();
        }

        public RenameColumnOperation RenameColumnOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel, string newName)
        {
            //string tableName = metadata.GetTableNameForClass(classModel);

            //var renameColumnOperation = new RenameColumnOperation(tableName, )

            throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        private ColumnModel CreateColumnModel(EdmProperty property)
        {
            string columnName = property.Name;
            bool columnNullable = property.Nullable;
            int? columnMaxLength = property.MaxLength;
            byte? columnPrecision = property.Precision;
            byte? columnScale = property.Scale;
            StoreGeneratedPattern columnStoreGeneratedPattern = property.StoreGeneratedPattern;
            var columnStoreType = property.TypeName;

            //var entityType
            //    = modelMetadata.StoreItemCollection
            //                   .OfType<EntityType>()
            //                   .Single(et => et.Name.EqualsIgnoreCase(entitySetName));

            var typeUsage = property.TypeUsage;
            //var typeUsage = modelMetadata.ProviderManifest.GetEdmType(property.TypeUsage);

            //var defaultStoreTypeName = modelMetadata.ProviderManifest.GetStoreType(typeUsage).EdmType.Name;


            var column
                = new ColumnModel(property.PrimitiveType.PrimitiveTypeKind, typeUsage)
                //= new ColumnModel(((PrimitiveType)property.TypeUsage.EdmType).PrimitiveTypeKind, typeUsage)
                {
                    Name = columnName,
                    IsNullable = columnNullable,
                    MaxLength = columnMaxLength,
                    Precision = columnPrecision,
                    Scale = columnScale,
                    StoreType = columnStoreType
                    //StoreType
                    //    = !string.Equals(columnStoreType, defaultStoreTypeName, StringComparison.OrdinalIgnoreCase)
                    //          ? columnStoreType
                    //          : null
                };

            column.IsIdentity = columnStoreGeneratedPattern.HasFlag(StoreGeneratedPattern.Identity);
            //TODO: check if identity type is valid type (EdmModelDiff does this)
            //&& _validIdentityTypes.Contains(column.Type);

            Facet facet;
            if (typeUsage.Facets.TryGetValue(DbProviderManifest.FixedLengthFacetName, true, out facet)
                && facet.Value != null
                && (bool)facet.Value)
            {
                column.IsFixedLength = true;
            }

            if (typeUsage.Facets.TryGetValue(DbProviderManifest.UnicodeFacetName, true, out facet)
                && facet.Value != null
                && !(bool)facet.Value)
            {
                column.IsUnicode = false;
            }

            var isComputed = columnStoreGeneratedPattern.HasFlag(StoreGeneratedPattern.Computed);

            if ((column.Type == PrimitiveTypeKind.Binary)
                && (typeUsage.Facets.TryGetValue(DbProviderManifest.MaxLengthFacetName, true, out facet)
                    && (facet.Value is int)
                    && ((int)facet.Value == 8))
                && isComputed)
            {
                column.IsTimestamp = true;
            }

            return column;
        }
        #endregion
    }
}
