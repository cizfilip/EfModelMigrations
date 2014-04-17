using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static T SingleOrThrow<T>(this IEnumerable<T> enumerable,
            Func<Exception> noneException, 
            Func<Exception> moreThanOneException)
        {
            T item = enumerable.FirstOrDefault();

            if (item == null)
                throw noneException();

            if (enumerable.Skip(1).Any())
                throw moreThanOneException();

            return item;
        }

        //TODO: pouzivat vice v projektu
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var t in enumerable)
            {
                action(t);
            }
        }

        public static string Uniquify(this IEnumerable<string> inputStrings, string targetString)
        {
            var uniqueString = targetString;
            var i = 0;

            while (inputStrings.Any(n => n.EqualsOrdinal(uniqueString)))
            {
                uniqueString = targetString + ++i;
            }

            return uniqueString;
        }
    }

   
}
