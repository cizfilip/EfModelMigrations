using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure.EntityFramework.Edmx;
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
using System.Xml.Linq;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    //TODO: handlovat dobre vyjimky, poresit problem kdyz je migrace slozena s takovych transformaci ktere nam zabrani v mapovani (napr. CreateClass a RemoveClass te same tridy v jedne migraci)
    public class DbMigrationOperationBuilder : IDbMigrationOperationBuilder
    {
        private string modelNamespace;
        private XDocument oldModel;
        private XDocument newModel;

        public DbMigrationOperationBuilder(string modelNamespace, XDocument oldModel, XDocument newModel)
        {
            this.modelNamespace = modelNamespace;
            this.oldModel = oldModel;
            this.newModel = newModel;
        }

        #region IDbMigrationOperationBuilder implementation
        
        //TODO: Co kdyz ma vytvarena trida nejake vazby?? - Momentalne neni podporovano
        public CreateTableOperation CreateTableOperation(string className)
        {
            return CreateTableOperationInternal(newModel, GetFullClassName(className));
        }

        public DropTableOperation DropTableOperation(string className)
        {
            string classFullName = GetFullClassName(className);

            string tableName = oldModel.GetTableName(classFullName);

            var inverseOperation = CreateTableOperationInternal(oldModel, classFullName);
            var dropTableOperation = new DropTableOperation(tableName, inverseOperation);
                      
            return dropTableOperation;
        }

        public AddColumnOperation AddColumnOperation(string className, string propertyName)
        {
            return AddColumnOperationInternal(newModel, GetFullClassName(className), propertyName);
        }

        public DropColumnOperation DropColumnOperation(string className, string propertyName)
        {
            string classFullName = GetFullClassName(className);

            string tableName = oldModel.GetTableName(classFullName);

            string columnName = oldModel.GetColumnName(classFullName, propertyName);

            var inverseOperation = AddColumnOperationInternal(oldModel, classFullName, propertyName);
            var dropColumnOperation = new DropColumnOperation(tableName, columnName, inverseOperation);

            return dropColumnOperation;
        }

        public RenameTableOperation RenameTableOperation(string oldClassName, string newClassName)
        {
            string oldTableName = oldModel.GetTableName(GetFullClassName(oldClassName));

            string newTableName = newModel.GetTableNameWithoutSchema(GetFullClassName(newClassName));

            //oldTableName is with schema, newName is NOT
            var renameTableOperation = new RenameTableOperation(oldTableName, newTableName);

            return renameTableOperation;
        }

        public RenameColumnOperation RenameColumnOperation(string className, string oldPropertyName, string newPropertyName)
        {
            string classFullName = GetFullClassName(className);

            string tableName = newModel.GetTableName(classFullName);

            string oldColumnName = oldModel.GetColumnName(classFullName, oldPropertyName);

            string newColumnName = newModel.GetColumnName(classFullName, newPropertyName);

            var renameColumnOperation = new RenameColumnOperation(tableName, oldColumnName, newColumnName);

            return renameColumnOperation;
        }

        #endregion

        #region Private methods

        private CreateTableOperation CreateTableOperationInternal(XDocument edmx, string classFullName)
        {
            //get table name
            string tableName = edmx.GetTableName(classFullName);
            CreateTableOperation createTableOperation = new CreateTableOperation(tableName);

            //map properties to colums 
            edmx.GetColumnModelsForClass(classFullName)
                .Each(c => createTableOperation.Columns.Add(c));

            //add primary keys
            var addPrimaryKeyOperation = new AddPrimaryKeyOperation();
            edmx.GetTableKeyColumnNamesForClass(classFullName)
                .Each(k => addPrimaryKeyOperation.Columns.Add(k));

            createTableOperation.PrimaryKey = addPrimaryKeyOperation;

            return createTableOperation;
        }

        private AddColumnOperation AddColumnOperationInternal(XDocument edmx, string classFullName, string propertyName)
        {
            string tableName = edmx.GetTableName(classFullName);

            string columnName = edmx.GetColumnName(classFullName, propertyName);

            var columnModel = edmx.GetColumnModelsForClass(classFullName)
                .Single(c => c.Name.EqualsOrdinalIgnoreCase(columnName));

            var addColumnOperation = new AddColumnOperation(tableName, columnModel);

            return addColumnOperation;
        }

        private string GetFullClassName(string className)
        {
            return modelNamespace + "." + className;
        }

        #endregion
    }
}
