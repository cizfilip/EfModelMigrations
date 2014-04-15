using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Extensions;
using EfModelMigrations.Resources;
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
        public bool TryFindModelMigrationsConfigurationType(Assembly assembly, out Type foundType)
        {
            return TryFindType(assembly, typeof(ModelMigrationsConfigurationBase), out foundType);
        }

        public bool TryFindDbMigrationsConfigurationType(Assembly assembly, out Type foundType)
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

            return TryMatchType(types, baseType, out foundType, derivedTypeName);
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

        private bool TryMatchType(IEnumerable<Type> types, Type baseType, out Type foundType, string derivedTypeName = null)
        {
            if (!string.IsNullOrEmpty(derivedTypeName))
            {
                types = types.Where(t => t.Name.EqualsOrdinalIgnoreCase(derivedTypeName));
            }

            var typesCount = types.Count();
            if (typesCount > 1)
            {
                throw new ModelMigrationsException(Strings.TypeFinder_MultipleTypesFound(baseType.FullName));
            }

            if (typesCount == 0)
            {
                foundType = null;
                return false;
            }

            foundType = types.Single();

            return true;
        }
    }
}
