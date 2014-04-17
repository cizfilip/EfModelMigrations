using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions
{
    public static class IndexAttributeExtensions
    {
        public static string GetDefaultNameIfRequired(this IndexAttribute index, IEnumerable<string> columns)
        {
            Check.NotNull(index, "index");

            if (string.IsNullOrEmpty(index.Name))
            {
                return IndexOperation.BuildDefaultName(columns);
            }
            
            return index.Name;
        }

        public static IndexAttribute CopyWithNameAndOrder(this IndexAttribute index, string name, int order)
        {
            Check.NotNull(index, "index");
            Check.NotEmpty(name, "name");

            var indexToReturn = new IndexAttribute(name, order);

            if (index.IsUniqueConfigured)
                indexToReturn.IsUnique = index.IsUnique;
            if (index.IsClusteredConfigured)
                indexToReturn.IsClustered = index.IsClustered;

            return indexToReturn;
        }


    }
}
