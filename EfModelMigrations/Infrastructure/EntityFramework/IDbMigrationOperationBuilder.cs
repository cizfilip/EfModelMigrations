using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public interface IDbMigrationOperationBuilder
    {
        //create/remove table
        CreateTableOperation CreateTableOperation(string className);
        DropTableOperation DropTableOperation(string className);

        //add/remove column
        AddColumnOperation AddColumnOperation(string className, string propertyName);
        DropColumnOperation DropColumnOperation(string className, string propertyName);

        //Rename operatoins
        RenameTableOperation RenameTableOperation(string oldClassName, string newClassName);
        RenameColumnOperation RenameColumnOperation(string className, string oldPropertyName, string newPropertyName);

        IEnumerable<RenameColumnOperation> RenameColumnOperationsForJoinComplexType(string complexTypeName, string className);


        //Relations
        IEnumerable<MigrationOperation> OneToOnePrimaryKeyRelationOperations(string principalClassName, string dependentClassName, bool willCascadeOnDelete);
        IEnumerable<MigrationOperation> OneToOneForeignKeyRelationOperations(string principalClassName, string dependentClassName, bool isDependentRequired, string[] foreignKeyColumnNames, bool willCascadeOnDelete);
        IEnumerable<MigrationOperation> OneToManyRelationOperations(string principalClassName, string dependentClassName, bool isDependentRequired, string[] foreignKeyColumnNames, bool willCascadeOnDelete);
        IEnumerable<MigrationOperation> ManyToManyRelationOperations(string principalClassName, string dependentClassName, string tableName, string[] leftKeyColumnNames, string[] rightKeyColumnNames);


        //Extract
        IEnumerable<MigrationOperation> ExtractTable(string fromClass, string newClass, string[] properties, string[] foreignKeyNames, bool willCascadeOnDelete);
    }
}
