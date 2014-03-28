using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Model
{
    public sealed class ManyToManyJoinTable
    {
        public string TableName { get; private set; }

        public string[] SourceForeignKeyColumns { get; private set; }

        public string[] TargetForeignKeyColumns { get; private set; }

        public ManyToManyJoinTable(string tableName, string[] sourceForeignKeyColumns, string[] targetForeignKeyColumns)
        {
            this.SourceForeignKeyColumns = sourceForeignKeyColumns;
            this.TargetForeignKeyColumns = targetForeignKeyColumns;
        }
    }
}
