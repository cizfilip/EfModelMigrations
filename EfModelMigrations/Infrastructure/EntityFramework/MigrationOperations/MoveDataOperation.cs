using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    public class MoveDataOperation : MigrationOperation
    {
        public MoveDataModel From { get; set; }
        public MoveDataModel To { get; set; }

        public MoveDataOperation(object anonymousArguments = null)
            :base(anonymousArguments)
        {
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }

    public class MoveDataModel
    {
        public string TableName { get; set; }

        public string[] ColumnNames { get; set; }

        public MoveDataModel(string tableName, string[] columns)
        {
            this.TableName = tableName;
            this.ColumnNames = columns;
        }
    }
}
