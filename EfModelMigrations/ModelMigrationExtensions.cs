using EfModelMigrations.Infrastructure.CodeModel;
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
            //TODO: Dat do classcodemodelu namespace atd....
            migration.AddTransformation(
                new CreateClassTransformation(
                    new ClassCodeModel()
                    {
                        Name = className,
                        Properties = ConvertObjectToPropertyModel(properties)
                    })
                );
        }

        public static void RemoveClass(this ModelMigration migration, string className)
        {
            migration.AddTransformation(new RemoveClassTransformation(className));
        }


        public static void AddProperty(this ModelMigration migration, string className, object property)
        {
            migration.AddTransformation(new AddPropertyTransformation(className, ConvertObjectToPropertyModel(property).Single()));
        }

        public static void RemoveProperty(this ModelMigration migration, string className, string propertyName)
        {
            migration.AddTransformation(new RemovePropertyTransformation(className, propertyName));
        }


        private static IEnumerable<PropertyCodeModel> ConvertObjectToPropertyModel(object properties)
        {
            foreach (var property in properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                yield return new PropertyCodeModel()
                {
                    Name = property.Name,
                    Type = property.GetGetMethod().Invoke(properties, new object[] { }) as string,
                    Visibility = CodeModelVisibility.Public
                };
            }
        }
    }
}
