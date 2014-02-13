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
                new CreateClassTransformation(className, ConvertObjectToPropertyModel(properties))
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

        public static void RenameClass(this ModelMigration migration, string oldClassName, string newClassName)
        {
            migration.AddTransformation(new RenameClassTransformation(oldClassName, newClassName));
        }

        public static void RenameProperty(this ModelMigration migration, string className, string oldPropertyName, string newPropertyName)
        {
            migration.AddTransformation(new RenamePropertyTransformation(className, oldPropertyName, newPropertyName));
        }

        public static void ExtractComplexType(this ModelMigration migration, string className, string complexTypeName, string[] propertiesToExtract)
        {
            migration.AddTransformation(new ExtractComplexTypeTransformation(className, complexTypeName, propertiesToExtract, NavigationPropertyCodeModel.Default(complexTypeName)));
        }

        public static void JoinComplexType(this ModelMigration migration, string complexTypeName, string className)
        {
            migration.AddTransformation(new JoinComplexTypeTransformation(complexTypeName, className));
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
