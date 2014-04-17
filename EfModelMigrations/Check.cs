using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EfModelMigrations
{
    //TODO: pridat Check.<method> napric celym projektem - hlavne u public method
    internal class Check
    {
        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static string NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(Strings.Check_NotEmpty(parameterName));
            }

            return value;
        }

        public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (!value.Any())
            {
                throw new ArgumentException(Strings.Check_NotNullOrEmpty(parameterName));
            }

            return value;
        }
    }
}
