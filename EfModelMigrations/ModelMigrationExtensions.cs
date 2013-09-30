using EfModelMigrations.Infrastructure.Model;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations
{
    public static class ModelMigrationExtensions
    {
        public static void CreateClass(this ModelMigration migration, string className, object properties)
        {
            migration.AddTransformation(
                new CreateClassTransformation(className, ConvertObjectToPropertyModel(properties))
                );
        }



        public static void RemoveClass(this ModelMigration migration, string className)
        {
            migration.AddTransformation(
                new RemoveClassTransformation(className)
                );
        }


        private static IEnumerable<PropertyModel> ConvertObjectToPropertyModel(object properties)
        {
            foreach (var property in properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                yield return new PropertyModel()
                {
                    Name = property.Name,
                    Type = property.GetGetMethod().Invoke(properties, new object[] { }) as string
                };
            }
        }
    }
}
