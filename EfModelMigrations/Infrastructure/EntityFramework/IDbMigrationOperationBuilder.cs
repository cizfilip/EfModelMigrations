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
        CreateTableOperation CreateTableOperation(ClassCodeModel classModel);
        DropTableOperation DropTableOperation(ClassCodeModel classModel);

        //add/remove column
        AddColumnOperation AddColumnOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel);
        DropColumnOperation DropColumnOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel);

        //Rename operatoins
        RenameTableOperation RenameTableOperation(ClassCodeModel classModel, string newTableName);
        RenameColumnOperation RenameColumnOperation(ClassCodeModel classModel, PropertyCodeModel propertyModel, string newName);
    }
}
