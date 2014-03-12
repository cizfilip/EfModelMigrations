using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.CodeModel.Builders;
using EfModelMigrations.Transformations;
using EfModelMigrations.Transformations.Model;
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
        public static void CreateClass<TProps>(this ModelMigration migration, string className, Func<ScalarPropertyBuilder, TProps> propertiesAction)
        {
            //TODO: Dat do classcodemodelu namespace atd....
            migration.AddTransformation(
                new CreateClassTransformation(className, ConvertObjectToPropertyModel(propertiesAction(new ScalarPropertyBuilder())))
                );
        }

        public static void RemoveClass(this ModelMigration migration, string className)
        {
            migration.AddTransformation(new RemoveClassTransformation(className));
        }


        public static void AddProperty(this ModelMigration migration, string className, string propertyName, Func<ScalarPropertyBuilder, ScalarProperty> propertyAction)
        {
            var scalarProperty = propertyAction(new ScalarPropertyBuilder());

            scalarProperty.Name = propertyName;
            migration.AddTransformation(new AddPropertyTransformation(className, scalarProperty));
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
            migration.AddTransformation(new ExtractComplexTypeTransformation(className, complexTypeName, propertiesToExtract, new NavigationPropertyBuilder().One(complexTypeName)));
        }

        public static void JoinComplexType(this ModelMigration migration, string complexTypeName, string className)
        {
            migration.AddTransformation(new JoinComplexTypeTransformation(complexTypeName, className));
        }

        public static void ExtractClass(this ModelMigration migration, string newClassName, string fromClassName, string[] propertiesToExtract, string[] foreignKeyColumns)
        {
            migration.AddTransformation(new ExtractClassTransformation(fromClassName, propertiesToExtract, newClassName, foreignKeyColumns));
        }

        //Associations
        public static AssociationBuilder Association(this ModelMigration migration)
        {
            return new AssociationBuilder(migration);
        }


        
        
        
        private static IEnumerable<ScalarProperty> ConvertObjectToPropertyModel<TProps>(TProps properties)
        {
            var propertiesOnObject = properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(
                p => !p.GetIndexParameters().Any());

            foreach (var property in propertiesOnObject)
            {
                var scalarProperty = property.GetValue(properties) as ScalarProperty;

                if (scalarProperty == null)
                    throw new ModelMigrationsException("Cannot retrieve property definition from migration!"); // TODO: string do resourcu

                if (string.IsNullOrWhiteSpace(scalarProperty.Name))
                {
                    scalarProperty.Name = property.Name;
                }

                yield return scalarProperty;
            }
        }
    }


   
}
