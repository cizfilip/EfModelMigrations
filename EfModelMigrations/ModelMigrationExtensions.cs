using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.CodeModel.Builders;
using EfModelMigrations.Transformations;
using EfModelMigrations.Transformations.Model;
using System.Data.Entity.Core.Metadata.Edm;
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
        public static void CreateClass<TProps>(this IModelMigration migration, string className, Func<ScalarPropertyBuilder, TProps> propertiesAction)
        {
            //TODO: Dat do classcodemodelu namespace atd....
            ((ModelMigration)migration).AddTransformation(
                new CreateClassTransformation(className, ConvertObjectToPropertyModel(propertiesAction(new ScalarPropertyBuilder())))
                );
        }

        public static void RemoveClass(this IModelMigration migration, string className)
        {
            ((ModelMigration)migration).AddTransformation(new RemoveClassTransformation(className));
        }


        public static void AddProperty(this IModelMigration migration, string className, string propertyName, Func<ScalarPropertyBuilder, ScalarPropertyCodeModel> propertyAction)
        {
            var scalarProperty = propertyAction(new ScalarPropertyBuilder());

            scalarProperty.Name = propertyName;
            ((ModelMigration)migration).AddTransformation(new AddPropertyTransformation(className, scalarProperty));
        }

        public static void RemoveProperty(this IModelMigration migration, string className, string propertyName)
        {
            ((ModelMigration)migration).AddTransformation(new RemovePropertyTransformation(className, propertyName));
        }

        public static void RenameClass(this IModelMigration migration, string oldClassName, string newClassName)
        {
            ((ModelMigration)migration).AddTransformation(new RenameClassTransformation(oldClassName, newClassName));
        }

        public static void RenameProperty(this IModelMigration migration, string className, string oldPropertyName, string newPropertyName)
        {
            ((ModelMigration)migration).AddTransformation(new RenamePropertyTransformation(className, oldPropertyName, newPropertyName));
        }

        public static void ExtractComplexType(this IModelMigration migration, string className, string complexTypeName, string[] propertiesToExtract)
        {
            ((ModelMigration)migration).AddTransformation(new ExtractComplexTypeTransformation(className, complexTypeName, propertiesToExtract, new NavigationPropertyCodeModel(complexTypeName)));
        }

        public static void JoinComplexType(this IModelMigration migration, string complexTypeName, string className)
        {
            ((ModelMigration)migration).AddTransformation(new JoinComplexTypeTransformation(complexTypeName, className));
        }

        public static void ExtractClass(this IModelMigration migration, string newClassName, string fromClassName, string[] propertiesToExtract, string[] foreignKeyColumns)
        {
            ((ModelMigration)migration).AddTransformation(new ExtractClassTransformation(fromClassName, propertiesToExtract, newClassName, foreignKeyColumns));
        }


        //Associations
        //public static AssociationBuilder Association(this IModelMigration migration)
        //{
        //    return new AssociationBuilder((ModelMigration)migration);
        //}

        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration, 
            string principal, 
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty, 
            string dependent, 
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty, 
            bool bothRequired = false, 
            bool? willCascadeOnDelete = null)
        {
            var principalMultiplicity = bothRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;

            ((ModelMigration)migration).AddTransformation(new AddOneToOnePrimaryKeyAssociationTransformation(
                new AssociationMemberInfo(principal, principalMultiplicity, principalNavigationProperty(new OneNavigationPropertyBuilder(dependent))),
                new AssociationMemberInfo(dependent, RelationshipMultiplicity.One, dependentNavigationProperty(new OneNavigationPropertyBuilder(principal))),
                willCascadeOnDelete));
                
        }
        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration,
            string principal,
            string dependent,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            bool bothRequired = false,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOnePrimaryKeyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, bothRequired, willCascadeOnDelete);
        }
        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration, 
            string principal,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty, 
            string dependent, 
            bool bothRequired = false,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOnePrimaryKeyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, bothRequired, willCascadeOnDelete);
        }


        public static void AddOneToOneForeignKeyAssociation(this IModelMigration migration,
            string principal,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames,
            bool principalRequired = false,
            bool dependentRequired = true,
            bool? willCascadeOnDelete = null)
        {
            var principalMultiplicity = principalRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;
            var dependentMultiplicity = dependentRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;

            ((ModelMigration)migration).AddTransformation(new AddOneToOneForeignKeyAssociationTransformation(
                new AssociationMemberInfo(principal, principalMultiplicity, principalNavigationProperty(new OneNavigationPropertyBuilder(dependent))),
                new AssociationMemberInfo(dependent, dependentMultiplicity, dependentNavigationProperty(new OneNavigationPropertyBuilder(principal))),
                dependentFkNames,
                willCascadeOnDelete));
        }
        public static void AddOneToOneForeignKeyAssociation(this IModelMigration migration,
            string principal,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            string[] dependentFkNames,
            bool principalRequired = false,
            bool dependentRequired = true,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOneForeignKeyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, dependentFkNames, principalRequired, dependentRequired, willCascadeOnDelete);
        }
        public static void AddOneToOneForeignKeyAssociation(this IModelMigration migration,
            string principal,
            string dependent,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames,
            bool principalRequired = false,
            bool dependentRequired = true,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOneForeignKeyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, dependentFkNames, principalRequired, dependentRequired, willCascadeOnDelete);
        }

        public static void AddManyToOneAssociation(this IModelMigration migration,
            string principal,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null)
        {
            var principalMultiplicity = principalRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;

            ((ModelMigration)migration).AddTransformation(new AddOneToManyAssociationTransformation(
                new AssociationMemberInfo(principal, principalMultiplicity, principalNavigationProperty(new ManyNavigationPropertyBuilder(dependent))),
                new AssociationMemberInfo(dependent, RelationshipMultiplicity.Many, dependentNavigationProperty(new OneNavigationPropertyBuilder(principal))),
                dependentFkNames,
                willCascadeOnDelete));
        }
        public static void AddManyToOneAssociation(this IModelMigration migration,
            string principal,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            string[] dependentFkNames,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null)
        {
            AddManyToOneAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, dependentFkNames, principalRequired, willCascadeOnDelete);
        }
        public static void AddManyToOneAssociation(this IModelMigration migration,
            string principal,
            string dependent,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null)
        {
            AddManyToOneAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, dependentFkNames, principalRequired, willCascadeOnDelete);
        }


        public static void AddManyToManyAssociation(this IModelMigration migration,
            string principal,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            ManyToManyJoinTable joinTable)
        {
            ((ModelMigration)migration).AddTransformation(new AddManyToManyAssociationTransformation(
                new AssociationMemberInfo(principal, RelationshipMultiplicity.Many, principalNavigationProperty(new ManyNavigationPropertyBuilder(dependent))),
                new AssociationMemberInfo(dependent, RelationshipMultiplicity.Many, dependentNavigationProperty(new ManyNavigationPropertyBuilder(principal))),
                joinTable));
        }
        public static void AddManyToManyAssociation(this IModelMigration migration,
            string principal,
            string dependent,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            ManyToManyJoinTable joinTable)
        {
            AddManyToManyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, joinTable);
        }
        public static void AddManyToManyAssociation(this IModelMigration migration,
            string principal,
            Func<NavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            ManyToManyJoinTable joinTable)
        {
            AddManyToManyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, joinTable);
        }

        
        private static IEnumerable<ScalarPropertyCodeModel> ConvertObjectToPropertyModel<TProps>(TProps properties)
        {
            var propertiesOnObject = properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(
                p => !p.GetIndexParameters().Any());

            foreach (var property in propertiesOnObject)
            {
                var scalarProperty = property.GetValue(properties) as ScalarPropertyCodeModel;

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
