using EfModelMigrations.Exceptions;
using EfModelMigrations.Resources;
using System;

namespace EfModelMigrations.Extensions
{
    public static class TypeExtensions
    {
        public static T CreateInstance<T>(this Type type, object[] constructorParameters = null)
        {
            try
            {
                return (T)Activator.CreateInstance(type, constructorParameters);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(Strings.CannotCreateInstance(type.Name), e);
            }
        }
    }
}
