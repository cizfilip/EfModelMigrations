using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Extensions
{
    internal static class StringExtensions
    {
        public static bool EqualsOrdinalIgnoreCase(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsOrdinal(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.Ordinal);
        }

        public static string RemoveFromEnd(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }
    }
}
