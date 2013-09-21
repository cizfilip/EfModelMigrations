using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Utilities
{
    public class TypeFinder
    {
        //well known types
        public bool FindModelMigrationsConfigurationType(Assembly assembly, out Type foundType)
        {
            return TryFindType(assembly, typeof(ModelMigrationsConfigurationBase), out foundType);
        }

        public bool FindDbMigrationsConfigurationType(Assembly assembly, out Type foundType)
        {
            return TryFindType(assembly, typeof(DbMigrationsConfiguration), out foundType);
        }

        public bool TryFindDbContextType(Assembly assembly, out Type foundType)
        {
            return TryFindType(assembly, typeof(DbContext), out foundType);
        }

        public bool TryFindType(Assembly assembly, Type baseType, out Type foundType, string derivedTypeName = null)
        {
            var types = FindTypes(assembly, baseType);

            if (!string.IsNullOrEmpty(derivedTypeName))
            {
                types = types.Where(t => string.Equals(t.Name, derivedTypeName, StringComparison.OrdinalIgnoreCase));
            }

            var typesCount = types.Count();
            if (typesCount > 1)
            {
                throw new ModelMigrationsException(string.Format(Resources.TypeFinder_MultipleTypesFound, assembly.FullName));
            }

            if (typesCount == 0)
            {
                foundType = null;
                return false;
            }

            foundType = types.Single();

            return true;
        }

        public Type FindType(Assembly assembly, Type baseType, string derivedTypeName = null)
        {
            Type foundType;

            if (!TryFindType(assembly, baseType, out foundType, derivedTypeName))
            {
                throw new ModelMigrationsException(string.Format(Resources.TypeFinder_NoTypesFound, assembly.FullName));
            }

            return foundType;
        }

        public IEnumerable<Type> FindTypes(Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => FindTypes(a, baseType));
        }

        public IEnumerable<Type> FindTypes(Assembly assembly, Type baseType)
        {
            var types = assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && baseType != t);

            return types.ToList();
        }
    }
}
