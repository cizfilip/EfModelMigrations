using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    internal class ForeignKeyIndexBuilder
    {

        public static bool TryBuild(string tableName, IEnumerable<EdmProperty> columns, out ConsolidatedIndex fkIndex)
        {
            Check.NotNullOrEmpty(columns, "columns");
            try
            {
                var consolidatedIndexes = ConsolidatedIndex.BuildIndexes(tableName, columns.Select(c => Tuple.Create(c.Name, c)));

                fkIndex = consolidatedIndexes.Where(c => c.Columns.SequenceEqual(columns.Select(col => col.Name))).Single();
                return true;
            }
            catch(Exception)
            {
                fkIndex = null;
                return false;
            }
        }

    }
}
