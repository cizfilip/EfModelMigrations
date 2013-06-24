using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mvc_evolution.PowerShell.Extensions
{
    internal static class AssemblyExtensions
    {
        public static Type GetContextType(this Assembly assembly)
        {
            Type dbContextType = typeof(DbContext);
            
            var contextTypes = assembly.GetAccessibleTypes()
                                           .Where(t => dbContextType.IsAssignableFrom(t))
                                           .ToList();

            if (contextTypes.Count > 1)
            {
                throw new InvalidOperationException("There is more than one DbContext in project!");
            }

            if (contextTypes.Count == 0)
            {
                throw new InvalidOperationException("No DbContext found in project!");
            }

            return contextTypes.First();
        }

        public static Type GetConfigurationType(this Assembly assembly)
        {

            Type configurationBaseType = typeof(DbMigrationsConfiguration);

            var contextTypes = assembly.GetAccessibleTypes()
                                           .Where(t => configurationBaseType.IsAssignableFrom(t))
                                           .ToList();

            if (contextTypes.Count > 1)
            {
                throw new InvalidOperationException("There is more than one DbMigrationsConfiguration in project!");
            }

            if (contextTypes.Count == 0)
            {
                throw new InvalidOperationException("No DbMigrationsConfiguration found in project!");
            }

            return contextTypes.First();
        }



        public static IEnumerable<Type> GetAccessibleTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // The exception is thrown if some types cannot be loaded in partial trust.
                // For our purposes we just want to get the types that are loaded, which are
                // provided in the Types property of the exception.
                return ex.Types.Where(t => t != null);
            }
        }
    }
}
