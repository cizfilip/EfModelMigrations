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
using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using EfModelMigrations.Exceptions;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    //TODO: az prestanu tuhle tridu pouzivat tak zaroven odstranit z projektu DbMigrationBuilderException
    //TODO: handlovat dobre vyjimky, poresit problem kdyz je migrace slozena s takovych transformaci ktere nam zabrani v mapovani (napr. CreateClass a RemoveClass te same tridy v jedne migraci)
    public class DbMigrationOperationBuilder : IDbMigrationOperationBuilder
    {
        private string modelNamespace;
        private XDocument oldModel;
        private XDocument newModel;

        public DbMigrationOperationBuilder(string modelNamespace, string oldModel, string newModel)
        {
            this.modelNamespace = modelNamespace;
            this.oldModel = XDocument.Parse(oldModel);
            this.newModel = XDocument.Parse(newModel);
        }

        #region IDbMigrationOperationBuilder implementation

        //TODO: Co kdyz ma vytvarena trida nejake vazby?? - Momentalne neni podporovano - a ani nebude ale musi se validovat ze zadne vazby nevzniknou
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


        //TODO: predelat tak aby se nemuselo vracet vic operaci
        public IEnumerable<RenameColumnOperation> RenameColumnOperationsForJoinComplexType(string complexTypeName, string className)
        {
            string tableName = newModel.GetTableName(GetFullClassName(className));

            foreach (var property in oldModel.GetComplexTypeProperties(complexTypeName))
            {
                yield return RenameColumnOperation(className, property, property);
            }
        }



        //1:1, 1:0, 0:1
        //TODO: stringy do resourcu
        public IEnumerable<MigrationOperation> OneToOnePrimaryKeyRelationOperations(string principalClassName, string dependentClassName, bool? willCascadeOnDelete)
        {
            string principalClassFullName = GetFullClassName(principalClassName);
            string dependentClassFullName = GetFullClassName(dependentClassName);

            string principalTableName = newModel.GetTableName(principalClassFullName);
            string dependentTableName = newModel.GetTableName(dependentClassFullName);


            var principalPrimaryKeyColumnsNames = newModel.GetTableKeyColumnNamesForClass(principalClassFullName).ToArray();
            var dependentPrimaryKeyColumnsNames = newModel.GetTableKeyColumnNamesForClass(dependentClassFullName).ToArray();

            //Primary keys validation
            if(principalPrimaryKeyColumnsNames.Length != dependentPrimaryKeyColumnsNames.Length) //Same count
            {
                throw new DbMigrationBuilderException(string.Format("Cannot create One to One relation between {0} and {1}, because they does not have same count of primary key columns", principalClassName, dependentClassName));
            }

            //same storage types
            if(!principalPrimaryKeyColumnsNames.Select(k => newModel.GetColumnStorageType(principalTableName, k))
                .SequenceEqual(dependentPrimaryKeyColumnsNames.Select(k => newModel.GetColumnStorageType(dependentTableName, k))))
            {
                throw new DbMigrationBuilderException(string.Format("Cannot create One to One relation between {0} and {1}, because their primary keys do not have same storage types", principalClassName, dependentClassName));
            }

            //Drop Identity operations
            if (dependentPrimaryKeyColumnsNames.Where(c => oldModel.IsColumnIdentity(c, dependentTableName)).Any())
            {
                //Validace pokud je treba dropovat identitu:
                //  1 - jen pokud jsou primarni klice jednoduche (nepodporuji zatim slozene klice ve vazbe)
                //  2 - jen pokud jsou oba klice typu int
                //  oboji plyne z toho ze neumim dropovat identitu pokud vyse uvedene neni splneno

                if(dependentPrimaryKeyColumnsNames.Length != 1)
                {
                    throw new DbMigrationBuilderException(string.Format("Cannot create One to One relation between {0} and {1}, because composite key is used and that is not suported yet.", principalClassName, dependentClassName));
                }

                var dependentPrimaryKeyColumnName = newModel.GetTableKeyColumnNamesForClass(dependentClassFullName).Single();
                if (newModel.GetColumnStorageType(dependentTableName, dependentPrimaryKeyColumnName) != "int")
                {
                    throw new DbMigrationBuilderException("Cannot create One to One relation between {0} and {1}, because primary keys are not int type. Only int primary keys are supported.");
                }

                var dropIdentityOperation = new DropIdentityOperation()
                {
                    PrincipalColumn = dependentPrimaryKeyColumnName,
                    PrincipalTable = dependentTableName
                };

                //TODO: pomoci ef metadat se tohle da nejspise krasne delat pomoci ForeignKeyDependents property na EntitySet :)
                foreach (var dependentColumn in newModel.GetDependentColumns(dependentTableName, dependentPrimaryKeyColumnName))
                {
                    dropIdentityOperation.DependentColumns.Add(new DependentColumn()
                    {
                        DependentTable = dependentColumn.Item1,
                        ForeignKeyColumn = dependentColumn.Item2
                    });

                }

                yield return dropIdentityOperation;

            }

            //CreateIndex operation
            yield return CreateIndexOperation(dependentTableName, dependentPrimaryKeyColumnsNames);

            //AddForeignKeyOperation
            yield return AddForeignKeyOperation(principalTableName, dependentTableName, principalPrimaryKeyColumnsNames, dependentPrimaryKeyColumnsNames, willCascadeOnDelete);
        }

        public IEnumerable<MigrationOperation> OneToOneForeignKeyRelationOperations(string principalClassName, string dependentClassName, bool isDependentRequired, string[] foreignKeyColumnNames, bool? willCascadeOnDelete)
        {
            return RelationWithForeignKeysOperations(principalClassName, dependentClassName, isDependentRequired, foreignKeyColumnNames, willCascadeOnDelete, true);
        }


        public IEnumerable<MigrationOperation> OneToManyRelationOperations(string principalClassName, string dependentClassName, bool isDependentRequired, string[] foreignKeyColumnNames, bool? willCascadeOnDelete)
        {
            return RelationWithForeignKeysOperations(principalClassName, dependentClassName, isDependentRequired, foreignKeyColumnNames, willCascadeOnDelete, false);
        }

        public IEnumerable<MigrationOperation> ManyToManyRelationOperations(string principalClassName, string dependentClassName, string tableName, string[] leftKeyColumnNames, string[] rightKeyColumnNames)
        {
            string principalClassFullName = GetFullClassName(principalClassName);
            string dependentClassFullName = GetFullClassName(dependentClassName);

            string principalTableName = newModel.GetTableName(principalClassFullName);
            string dependentTableName = newModel.GetTableName(dependentClassFullName);

            var principalPrimaryKeyColumnsNames = newModel.GetTableKeyColumnNamesForClass(principalClassFullName).ToArray();
            var dependentPrimaryKeyColumnsNames = newModel.GetTableKeyColumnNamesForClass(dependentClassFullName).ToArray();

            //Validation of principal pk and supplied fk names
            if (principalPrimaryKeyColumnsNames.Length != leftKeyColumnNames.Length)
            {
                throw new DbMigrationBuilderException(string.Format("Cannot create many to many relation between {0} and {1}, because count of supplied principal foreign keys is not same as principal primary keys count.", principalClassName, dependentClassName));
            }
            if (dependentPrimaryKeyColumnsNames.Length != rightKeyColumnNames.Length)
            {
                throw new DbMigrationBuilderException(string.Format("Cannot create many to many relation between {0} and {1}, because count of supplied dependent foreign keys is not same as dependent primary keys count.", principalClassName, dependentClassName));
            }

            string fullTableName = newModel.GetTableFullNameForTable(tableName);
            var joinTableColumnNames = leftKeyColumnNames.Concat(rightKeyColumnNames).ToArray();

            // create join table operation
            var createTableOperation = new CreateTableOperation(fullTableName);
            foreach (var column in joinTableColumnNames)
            {
                var columnModel = newModel.GetColumnModel(fullTableName, column);
                createTableOperation.Columns.Add(columnModel);
            }
            var addPrimaryKeyOperation = new AddPrimaryKeyOperation();
            joinTableColumnNames.Each(c => addPrimaryKeyOperation.Columns.Add(c));

            createTableOperation.PrimaryKey = addPrimaryKeyOperation;
            yield return createTableOperation;

            //indexes
            yield return CreateIndexOperation(fullTableName, leftKeyColumnNames);
            yield return CreateIndexOperation(fullTableName, rightKeyColumnNames);

            //foreign keys
            yield return AddForeignKeyOperation(principalTableName, fullTableName, principalPrimaryKeyColumnsNames, leftKeyColumnNames, true);
            yield return AddForeignKeyOperation(dependentTableName, fullTableName, dependentPrimaryKeyColumnsNames, rightKeyColumnNames, true);
        }


        public IEnumerable<MigrationOperation> ExtractTable(string fromClass, string newClass, string[] properties, string[] foreignKeyNames, bool willCascadeOnDelete)
        {
            yield return CreateTableOperationInternal(newModel, GetFullClassName(newClass));

            var relationOperations = RelationWithForeignKeysOperations(fromClass, newClass, true, foreignKeyNames, willCascadeOnDelete, true, false);
            foreach (var op in relationOperations)
            {
                yield return op;
            }

            yield return MoveDataOperation(fromClass, newClass, properties, foreignKeyNames);

            foreach (var property in properties)
            {
                yield return DropColumnOperation(fromClass, property);
            }
        }

        #endregion

        #region Private methods

        private MoveDataOperation MoveDataOperation(string fromClass, string toClass, string[] properties, string[] foreignKeyNames)
        {
            string fromClassFullName = GetFullClassName(fromClass);
            string toClassFullName = GetFullClassName(toClass);

            string fromTableName = newModel.GetTableName(fromClassFullName);
            string toTableName = newModel.GetTableName(toClassFullName);

            var fromTablePrimaryKeyColumnsNames = newModel.GetTableKeyColumnNamesForClass(fromClassFullName);
            var fromColumnNames = properties.Select(p => oldModel.GetColumnName(fromClassFullName, p)); //from columns se musi brat ze stareho modelu - v novem uz nejsou
            var fromColumns = fromTablePrimaryKeyColumnsNames.Concat(fromColumnNames).ToArray();

            var fromModel = new MoveDataModel(fromTableName, fromColumns);

            var toColumnNames = properties.Select(p => newModel.GetColumnName(toClassFullName, p));
            var toColumns = foreignKeyNames.Concat(toColumnNames).ToArray();

            var toModel = new MoveDataModel(toTableName, toColumns);

            var operation = new MoveDataOperation();
            operation.From = fromModel;
            operation.To = toModel;

            return operation;
        }

        private IEnumerable<MigrationOperation> RelationWithForeignKeysOperations(string principalClassName, string dependentClassName, bool isDependentRequired, string[] foreignKeyColumnNames, bool? willCascadeOnDelete, bool isIndexUnique, bool includeAddColumnsForForeignKey = true)
        {
            string principalClassFullName = GetFullClassName(principalClassName);
            string dependentClassFullName = GetFullClassName(dependentClassName);

            string principalTableName = newModel.GetTableName(principalClassFullName);
            string dependentTableName = newModel.GetTableName(dependentClassFullName);

            var principalPrimaryKeyColumnsNames = newModel.GetTableKeyColumnNamesForClass(principalClassFullName).ToArray();

            //Validation of principal pk and supplied fk names
            if (principalPrimaryKeyColumnsNames.Length != foreignKeyColumnNames.Length)
            {
                throw new DbMigrationBuilderException(string.Format("Cannot create relation between {0} and {1}, because count of supplied foreign keys is not same as principal primary keys count.", principalClassName, dependentClassName));
            }

            //Add foreign key columns
            if (includeAddColumnsForForeignKey)
            {
                for (int i = 0; i < foreignKeyColumnNames.Length; i++)
                {
                    var columnModel = newModel.GetColumnModel(principalTableName, principalPrimaryKeyColumnsNames[i]);
                    columnModel.Name = foreignKeyColumnNames[i];
                    columnModel.IsNullable = !isDependentRequired;

                    yield return new AddColumnOperation(dependentTableName, columnModel);
                }
            }

            //CreateIndex operation
            yield return CreateIndexOperation(dependentTableName, foreignKeyColumnNames, isIndexUnique);

            //AddForeignKeyOperation
            yield return AddForeignKeyOperation(principalTableName, dependentTableName, principalPrimaryKeyColumnsNames, foreignKeyColumnNames, willCascadeOnDelete);
        }

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

        private CreateIndexOperation CreateIndexOperation(string tableName, string[] columns, bool isUnique = false)
        {
            var createIndexOperation = new CreateIndexOperation()
            {
                Table = tableName,
                IsUnique = isUnique,
                IsClustered = false,
                Name = null
            };
            columns.Each(c => createIndexOperation.Columns.Add(c));
            return createIndexOperation;
        }

        private AddForeignKeyOperation AddForeignKeyOperation(string principalTable, string dependentTable, string[] principalColumns, string[] dependentColumns, bool? cascadeOnDelete)
        {
            var addForeignKeyOperation = new AddForeignKeyOperation()
            {
                CascadeDelete = cascadeOnDelete.HasValue ? cascadeOnDelete.Value : false,
                Name = null,
                PrincipalTable = principalTable,
                DependentTable = dependentTable
            };

            principalColumns.Each(c => addForeignKeyOperation.PrincipalColumns.Add(c));

            dependentColumns.Each(c => addForeignKeyOperation.DependentColumns.Add(c));

            return addForeignKeyOperation;
        }


        private string GetFullClassName(string className)
        {
            return modelNamespace + "." + className;
        }

        #endregion







        
    }
}
