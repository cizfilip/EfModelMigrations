using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.MigrationOperations
{
    //TODO: a co inverse???
    public class InsertFromOperation : MigrationOperation
    {
        public InsertDataModel From { get; set; }
        public InsertDataModel To { get; set; }

        public InsertFromOperation(object anonymousArguments = null)
            :base(anonymousArguments)
        {
        }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }

    public sealed class InsertDataModel
    {
        public string TableName { get; set; }

        public string[] ColumnNames { get; set; }

        public InsertDataModel(string tableName, string[] columns)
        {
            this.TableName = tableName;
            this.ColumnNames = columns;
        }
    }
}
