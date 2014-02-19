using EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Builders;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public class ExtendedSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(MigrationOperation migrationOperation)
        {
            if (migrationOperation is IdentityOperation)
            {
                Generate((IdentityOperation)migrationOperation);
            }    
        }

        protected virtual void Generate(IdentityOperation migrationOperation)
        {
            var operation = migrationOperation as IdentityOperation;
                bool switchingIdentityOn = operation is AddIdentityOperation;

                var tempPrincipalColumnName = "old_" + operation.PrincipalColumn;

                // 1. Drop all foreign key constraints that point to the primary key we are changing
                foreach (var item in operation.DependentColumns)
                {
                    Generate(new DropForeignKeyOperation
                    {
                        DependentTable = item.DependentTable,
                        PrincipalTable = operation.PrincipalTable,
                        DependentColumns = { item.ForeignKeyColumn }
                    });
                }

                // 2. Drop the primary key constraint
                Generate(new DropPrimaryKeyOperation { Table = operation.PrincipalTable });

                // 3. Rename the existing column (so that we can re-create the foreign key relationships later)
                Generate(new RenameColumnOperation(
                    operation.PrincipalTable,
                    operation.PrincipalColumn,
                    tempPrincipalColumnName));

                // 4. Add the new primary key column with the new identity setting
                Generate(new AddColumnOperation(
                    operation.PrincipalTable,
                    new ColumnBuilder().Int(
                        name: operation.PrincipalColumn,
                        nullable: false,
                        identity: switchingIdentityOn)));

                // 5. Update existing data so that previous foreign key relationships remain
                if (switchingIdentityOn)
                {
                    // If the new column is an identity column we need to update all 
                    // foreign key columns with the new values
                    foreach (var item in operation.DependentColumns)
                    {
                        Generate(new SqlOperation(
                            "UPDATE " + item.DependentTable +
                            " SET " + item.ForeignKeyColumn +
                                " = (SELECT TOP 1 " + operation.PrincipalColumn +
                                " FROM " + operation.PrincipalTable +
                                " WHERE " + tempPrincipalColumnName + " = " + item.DependentTable + "." + item.ForeignKeyColumn + ")"));
                    }
                }
                else
                {
                    // If the new column doesn’t have identity on then we can copy the old 
                    // values from the previous identity column
                    Generate(new SqlOperation(
                        "UPDATE " + operation.PrincipalTable +
                        " SET " + operation.PrincipalColumn + " = " + tempPrincipalColumnName + ";"));
                }

                // 6. Drop old primary key column
                Generate(new DropColumnOperation(
                    operation.PrincipalTable,
                    tempPrincipalColumnName));

                // 7. Add primary key constraint
                Generate(new AddPrimaryKeyOperation
                {
                    Table = operation.PrincipalTable,
                    Columns = { operation.PrincipalColumn }
                });

                // 8. Add back foreign key constraints
                foreach (var item in operation.DependentColumns)
                {
                    Generate(new AddForeignKeyOperation
                    {
                        DependentTable = item.DependentTable,
                        DependentColumns = { item.ForeignKeyColumn },
                        PrincipalTable = operation.PrincipalTable,
                        PrincipalColumns = { operation.PrincipalColumn }
                    });
                }
            
        }
    }
}
