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

        public string[] PrincipalForeignKeyColumns { get; private set; }

        public string[] DependentForeignKeyColumns { get; private set; }

        public ManyToManyJoinTable(string tableName, string[] principalForeignKeyColumns, string[] dependentForeignKeyColumns)
        {
            this.PrincipalForeignKeyColumns = principalForeignKeyColumns;
            this.DependentForeignKeyColumns = dependentForeignKeyColumns;
        }
    }
}
