using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EfModelMigrations.Infrastructure.EntityFramework.Edmx
{
    internal static class XContainerExtension
    {

        public static IEnumerable<XElement> Descendants(this XContainer container, IEnumerable<XName> name)
        {
            return name.SelectMany(container.Descendants);
        }

        public static IEnumerable<XElement> Descendants<T>(this IEnumerable<T> source, IEnumerable<XName> name)
            where T : XContainer
        {
            return name.SelectMany(n => source.SelectMany(c => c.Descendants(n)));
        }
    }
}
