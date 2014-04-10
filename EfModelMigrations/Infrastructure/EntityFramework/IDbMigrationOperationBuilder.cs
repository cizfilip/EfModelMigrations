using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    //TODO: asi dodelat support anotaci po vzoru efmodel differu
    public interface IDbMigrationOperationBuilder
    {
        EfModel OldModel { get; }

        EfModel NewModel { get; }

        //create/remove table
        CreateTableOperation CreateTableOperation(EntitySet storageEntitySet);
        DropTableOperation DropTableOperation(EntitySet storageEntitySet);

        //add/remove column
        AddColumnOperation AddColumnOperation(EntitySet storageEntitySet, EdmProperty column);
        DropColumnOperation DropColumnOperation(EntitySet storageEntitySet, EdmProperty column);

        //Rename operatoins
        RenameTableOperation RenameTableOperation(EntitySet oldStorageEntitySet, EntitySet newStorageEntitySet);
        RenameColumnOperation RenameColumnOperation(EntitySet storageEntitySet, EdmProperty oldColumn, EdmProperty newColumn);

        //Relations operations
        CreateIndexOperation TryBuildCreateIndexOperation(EntitySet storageEntitySet, IEnumerable<EdmProperty> columns);
        DropIndexOperation TryBuildDropIndexOperation(EntitySet storageEntitySet, IEnumerable<EdmProperty> columns);

        AddForeignKeyOperation AddForeignKeyOperation(ReferentialConstraint referentialConstraint);
        DropForeignKeyOperation DropForeignKeyOperation(ReferentialConstraint referentialConstraint);

        //Identity operations
        AddIdentityOperation TryBuildAddIdentityOperation(EntitySet storageEntitySet);
        DropIdentityOperation TryBuildDropIdentityOperation(EntitySet storageEntitySet);


        //Other
        //InsertFromOperation InsertFromOperation(EntitySet fromEntitySet, IEnumerable<EdmProperty> fromColumns, EntitySet toEntitySet, IEnumerable<EdmProperty> toColumns);

       
    }
}
