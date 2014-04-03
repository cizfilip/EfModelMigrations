using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Extensions
{
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            collection.Each(i => list.Add(i));
        }

    }
}
