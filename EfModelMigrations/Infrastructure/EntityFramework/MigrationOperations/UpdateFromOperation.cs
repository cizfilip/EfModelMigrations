using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public class UpdateFromOperation : MoveDataOperation<UpdateFromDataModel>
    {
        public UpdateFromOperation(MigrationOperation inverse, object anonymousArguments = null)
            : base(inverse, anonymousArguments)
        {
        }

        public UpdateFromOperation(object anonymousArguments = null)
            : base(null, anonymousArguments)
        {
        }
    }

    public sealed class UpdateFromDataModel
    {
        public string TableName { get; set; }

        public string[] JoinColumns { get; set; }

        public string[] ColumnNames { get; set; }

        public UpdateFromDataModel(string tableName, string[] columns, string[] joinColumns)
        {
            this.TableName = tableName;
            this.ColumnNames = columns;
            this.JoinColumns = joinColumns;
        }
    }
}
