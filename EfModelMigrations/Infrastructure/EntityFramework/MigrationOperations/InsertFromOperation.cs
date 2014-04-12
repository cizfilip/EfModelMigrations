using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public class InsertFromOperation : MoveDataOperation<InserFromDataModel>
    {
        public InsertFromOperation(MigrationOperation inverse, object anonymousArguments = null)
            : base(inverse, anonymousArguments)
        {
        }

        public InsertFromOperation(object anonymousArguments = null)
            : base(null, anonymousArguments)
        {
        }
    }

    public sealed class InserFromDataModel
    {
        public string TableName { get; set; }

        public string[] ColumnNames { get; set; }

        public InserFromDataModel(string tableName, string[] columns)
        {
            this.TableName = tableName;
            this.ColumnNames = columns;
        }
    }
}
