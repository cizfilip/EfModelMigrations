using EfModelMigrations.Exceptions;
using EfModelMigrations.Properties;
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
                throw new ModelMigrationsException(string.Format(Resources.CannotCreateInstance, type.Name), e);
            }
        }
    }
}
