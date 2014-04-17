using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public static class EntitySetExtensions
    {
        //TODO: mozna prevzit z EF tridu DbName a pouzivat ji
        public static string FullTableName(this EntitySet storageEntitySet)
        {
            Check.NotNull(storageEntitySet, "storageEntitySet");

            return string.Concat(storageEntitySet.Schema, ".", storageEntitySet.Table);
        }
    }
}
