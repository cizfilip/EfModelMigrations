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
using System.ComponentModel.DataAnnotations.Schema;

namespace EfModelMigrations
{
    public static class ModelMigrationExtensions
    {
        public static void CreateClass<TProps>(this IModelMigration migration, Func<ClassModelBuilder, ClassModel> classAction, Func<PrimitivePropertyBuilder, TProps> propertiesAction, string[] primaryKeys = null)
        {
            //TODO: Dat do classcodemodelu namespace atd....
            ((ModelMigration)migration).AddTransformation(
                new CreateClassTransformation(classAction(new ClassModelBuilder()), ConvertObjectToPrimitivePropertyModel(propertiesAction(new PrimitivePropertyBuilder())), primaryKeys)
                );
        }

        public static void RemoveClass(this IModelMigration migration, string className)
        {
            ((ModelMigration)migration).AddTransformation(new RemoveClassTransformation(className));
        }

        public static void AddProperty(this IModelMigration migration, string className, string propertyName, Func<PrimitivePropertyBuilder, PrimitiveMappingBuilder> propertyAction)
        {
            var property = propertyAction(new PrimitivePropertyBuilder()).Property;

            property.Name = propertyName;
            ((ModelMigration)migration).AddTransformation(new AddPropertyTransformation(className, property));
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

        public static void ExtractClass<TPks>(this IModelMigration migration, 
            string fromClassName, 
            string[] propertiesToExtract, 
            Func<ClassModelBuilder, ClassModel> newClassAction, 
            Func<PrimitivePropertyBuilder, TPks> primaryKeysAction = null, 
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> fromClassNavigationPropAction = null,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> newClassNavigationPropAction = null,
            string[] foreignKeyColumns = null)
        {
            var newClassModel = newClassAction(new ClassModelBuilder());
            NavigationPropertyCodeModel fromNavigationProp = fromClassNavigationPropAction != null ? fromClassNavigationPropAction(new OneNavigationPropertyBuilder(newClassModel.Name)) : null;
            NavigationPropertyCodeModel newNavigationProp = newClassNavigationPropAction != null ? newClassNavigationPropAction(new OneNavigationPropertyBuilder(fromClassName)) : null;

            IEnumerable<PrimitivePropertyCodeModel> primaryKeys = primaryKeysAction != null ? ConvertObjectToPrimitivePropertyModel(primaryKeysAction(new PrimitivePropertyBuilder())) : null;

            ((ModelMigration)migration).AddTransformation(
                new ExtractClassTransformation(fromClassName, propertiesToExtract, newClassModel, primaryKeys, fromNavigationProp, newNavigationProp, foreignKeyColumns)
            );
        }


        //Associations
        //public static AssociationBuilder Association(this IModelMigration migration)
        //{
        //    return new AssociationBuilder((ModelMigration)migration);
        //}

        //1:1 PK
        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration,
            string principal,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            bool bothRequired = false,
            bool? willCascadeOnDelete = null)
        {
            var principalMultiplicity = bothRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;

            ((ModelMigration)migration).AddTransformation(new AddOneToOnePrimaryKeyAssociationTransformation(
                new AssociationEnd(principal, principalMultiplicity, principalNavigationProperty(new OneNavigationPropertyBuilder(dependent))),
                new AssociationEnd(dependent, RelationshipMultiplicity.One, dependentNavigationProperty(new OneNavigationPropertyBuilder(principal))),
                willCascadeOnDelete));

        }
        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration,
            string principal,
            string dependent,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            bool bothRequired = false,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOnePrimaryKeyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, bothRequired, willCascadeOnDelete);
        }
        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration,
            string principal,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            bool bothRequired = false,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOnePrimaryKeyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, bothRequired, willCascadeOnDelete);
        }

        //1:1 FK
        public static void AddOneToOneForeignKeyAssociation(this IModelMigration migration,
            string principal,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames = null,
            bool principalRequired = false,
            bool dependentRequired = true,
            bool? willCascadeOnDelete = null)
        {
            var principalMultiplicity = principalRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;
            var dependentMultiplicity = dependentRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;

            ((ModelMigration)migration).AddTransformation(new AddOneToOneForeignKeyAssociationTransformation(
                new AssociationEnd(principal, principalMultiplicity, principalNavigationProperty(new OneNavigationPropertyBuilder(dependent))),
                new AssociationEnd(dependent, dependentMultiplicity, dependentNavigationProperty(new OneNavigationPropertyBuilder(principal))),
                dependentFkNames,
                willCascadeOnDelete));
        }
        public static void AddOneToOneForeignKeyAssociation(this IModelMigration migration,
            string principal,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            string[] dependentFkNames = null,
            bool principalRequired = false,
            bool dependentRequired = true,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOneForeignKeyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, dependentFkNames, principalRequired, dependentRequired, willCascadeOnDelete);
        }
        public static void AddOneToOneForeignKeyAssociation(this IModelMigration migration,
            string principal,
            string dependent,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames = null,
            bool principalRequired = false,
            bool dependentRequired = true,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOneForeignKeyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, dependentFkNames, principalRequired, dependentRequired, willCascadeOnDelete);
        }

        //1:N - ForeignKeyNames
        public static void AddOneToManyAssociation(this IModelMigration migration,
            string principal,
            Func<ManyNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames = null,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            var principalMultiplicity = principalRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;

            ((ModelMigration)migration).AddTransformation(new AddOneToManyAssociationTransformation(
                new AssociationEnd(principal, principalMultiplicity, principalNavigationProperty(new ManyNavigationPropertyBuilder(dependent))),
                new AssociationEnd(dependent, RelationshipMultiplicity.Many, dependentNavigationProperty(new OneNavigationPropertyBuilder(principal))),
                dependentFkNames,
                willCascadeOnDelete,
                foreignKeyIndex));
        }
        public static void AddOneToManyAssociation(this IModelMigration migration,
            string principal,
            Func<ManyNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            string[] dependentFkNames = null,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            AddOneToManyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, dependentFkNames, principalRequired, willCascadeOnDelete, foreignKeyIndex);
        }
        public static void AddOneToManyAssociation(this IModelMigration migration,
            string principal,
            string dependent,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames = null,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            AddOneToManyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, dependentFkNames, principalRequired, willCascadeOnDelete, foreignKeyIndex);
        }

        //1:N - ForeignKeyProperties
        public static void AddOneToManyAssociation<TProps>(this IModelMigration migration,
            string principal,
            Func<ManyNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            Func<ForeignKeyPropertyBuilder, TProps> dependentFkPropertiesAction,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            var principalMultiplicity = principalRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;

            ((ModelMigration)migration).AddTransformation(new AddOneToManyAssociationTransformation(
                new AssociationEnd(principal, principalMultiplicity, principalNavigationProperty(new ManyNavigationPropertyBuilder(dependent))),
                new AssociationEnd(dependent, RelationshipMultiplicity.Many, dependentNavigationProperty(new OneNavigationPropertyBuilder(principal))),
                ConvertObjectToForeignKeyPropertyModel(dependentFkPropertiesAction(new ForeignKeyPropertyBuilder())).ToArray(),
                willCascadeOnDelete,
                foreignKeyIndex));
        }
        public static void AddOneToManyAssociation<TProps>(this IModelMigration migration,
            string principal,
            Func<ManyNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<ForeignKeyPropertyBuilder, TProps> dependentFkPropertiesAction,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            AddOneToManyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, dependentFkPropertiesAction, principalRequired, willCascadeOnDelete, foreignKeyIndex);
        }
        public static void AddOneToManyAssociation<TProps>(this IModelMigration migration,
            string principal,
            string dependent,
            Func<OneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            Func<ForeignKeyPropertyBuilder, TProps> dependentFkPropertiesAction,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            AddOneToManyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, dependentFkPropertiesAction, principalRequired, willCascadeOnDelete, foreignKeyIndex);
        }

        //M:N
        public static void AddManyToManyAssociation(this IModelMigration migration,
            string source,
            Func<ManyNavigationPropertyBuilder, NavigationPropertyCodeModel> sourceNavigationProperty,
            string target,
            Func<ManyNavigationPropertyBuilder, NavigationPropertyCodeModel> targetNavigationProperty,
            ManyToManyJoinTable joinTable = null)
        {
            ((ModelMigration)migration).AddTransformation(new AddManyToManyAssociationTransformation(
                new AssociationEnd(source, RelationshipMultiplicity.Many, sourceNavigationProperty(new ManyNavigationPropertyBuilder(target))),
                new AssociationEnd(target, RelationshipMultiplicity.Many, targetNavigationProperty(new ManyNavigationPropertyBuilder(source))),
                joinTable));
        }
        public static void AddManyToManyAssociation(this IModelMigration migration,
            string source,
            string target,
            Func<ManyNavigationPropertyBuilder, NavigationPropertyCodeModel> targetNavigationProperty,
            ManyToManyJoinTable joinTable = null)
        {
            AddManyToManyAssociation(migration, source, _ => null, target, targetNavigationProperty, joinTable);
        }
        public static void AddManyToManyAssociation(this IModelMigration migration,
            string source,
            Func<ManyNavigationPropertyBuilder, NavigationPropertyCodeModel> sourceNavigationProperty,
            string target,
            ManyToManyJoinTable joinTable = null)
        {
            AddManyToManyAssociation(migration, source, sourceNavigationProperty, target, _ => null, joinTable);
        }


        //Remove association
        public static void RemoveOneToOnePrimaryKeyAssociation(this IModelMigration migration,
            string source,
            string sourceNavigationPropertyName,
            string target,
            string targetNavigationPropertyName)
        {
            ((ModelMigration)migration).AddTransformation(new RemoveOneToOneForeignKeyAssociationTransformation(
                new SimpleAssociationEnd(source, sourceNavigationPropertyName),
                new SimpleAssociationEnd(target, targetNavigationPropertyName)));
        }

        public static void RemoveOneToOneForeignKeyAssociation(this IModelMigration migration,
            string source,
            string sourceNavigationPropertyName,
            string target,
            string targetNavigationPropertyName)
        {
            ((ModelMigration)migration).AddTransformation(new RemoveOneToOneForeignKeyAssociationTransformation(
                new SimpleAssociationEnd(source, sourceNavigationPropertyName),
                new SimpleAssociationEnd(target, targetNavigationPropertyName)));
        }

        public static void RemoveOneToManyAssociation(this IModelMigration migration,
            string source,
            string sourceNavigationPropertyName,
            string target,
            string targetNavigationPropertyName,
            string[] foreignKeyPropertyNames = null)
        {
            ((ModelMigration)migration).AddTransformation(new RemoveOneToManyAssociationTransformation(
                new SimpleAssociationEnd(source, sourceNavigationPropertyName),
                new SimpleAssociationEnd(target, targetNavigationPropertyName),
                foreignKeyPropertyNames));
        }

        public static void RemoveManyToManyAssociation(this IModelMigration migration,
            string source,
            string sourceNavigationPropertyName,
            string target,
            string targetNavigationPropertyName)
        {
            ((ModelMigration)migration).AddTransformation(new RemoveManyToManyAssociationTransformation(
                new SimpleAssociationEnd(source, sourceNavigationPropertyName),
                new SimpleAssociationEnd(target, targetNavigationPropertyName)));
        }


        private static IEnumerable<PrimitivePropertyCodeModel> ConvertObjectToPrimitivePropertyModel<TProps>(TProps properties)
        {
            var propertiesOnObject = properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(
                p => !p.GetIndexParameters().Any());

            foreach (var property in propertiesOnObject)
            {
                var mappingBuilder = property.GetValue(properties) as PrimitiveMappingBuilder;

                var primitiveProperty = mappingBuilder != null ? mappingBuilder.Property : null;

                if (primitiveProperty == null)
                    throw new ModelMigrationsException("Cannot retrieve property definition from migration!"); // TODO: string do resourcu

                if (string.IsNullOrWhiteSpace(primitiveProperty.Name))
                {
                    primitiveProperty.Name = property.Name;
                }

                yield return primitiveProperty;
            }
        }

        private static IEnumerable<ForeignKeyPropertyCodeModel> ConvertObjectToForeignKeyPropertyModel<TProps>(TProps properties)
        {
            var propertiesOnObject = properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(
                p => !p.GetIndexParameters().Any());

            foreach (var property in propertiesOnObject)
            {
                var fkProperty = property.GetValue(properties) as ForeignKeyPropertyCodeModel;

                if (fkProperty == null)
                    throw new ModelMigrationsException("Cannot retrieve property definition from migration!"); // TODO: string do resourcu

                if (string.IsNullOrWhiteSpace(fkProperty.Name))
                {
                    fkProperty.Name = property.Name;
                }

                yield return fkProperty;
            }
        }
    }



}
