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
using EfModelMigrations.Resources;

namespace EfModelMigrations
{
    public static class ModelMigrationExtensions
    {
        public static void CreateClass(this IModelMigration migration, Func<IClassModelBuilder, ClassModel> classAction, Func<IPrimitivePropertyBuilder, object> propertiesAction, string[] primaryKeys = null)
        {
            Check.NotNull(migration, "migration");
            Check.NotNull(classAction, "classAction");
            Check.NotNull(propertiesAction, "propertiesAction");
            
            ((ModelMigration)migration).AddTransformation(
                new CreateClassTransformation(classAction(new ClassModelBuilder()), ConvertObjectToPrimitivePropertyModel(propertiesAction(new PrimitivePropertyBuilder())), primaryKeys)
                );
        }

        public static void RemoveClass(this IModelMigration migration, string className)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(className, "className");

            ((ModelMigration)migration).AddTransformation(new RemoveClassTransformation(className));
        }

        public static void AddProperty(this IModelMigration migration, string className, string propertyName, Func<IPrimitivePropertyBuilder, PrimitiveMappingBuilder> propertyAction)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(className, "className");
            Check.NotEmpty(propertyName, "propertyName");
            Check.NotNull(propertyAction, "propertyAction");

            var property = propertyAction(new PrimitivePropertyBuilder()).Property;

            property.Name = propertyName;
            ((ModelMigration)migration).AddTransformation(new AddPropertyTransformation(className, property));
        }

        public static void RemoveProperty(this IModelMigration migration, string className, string propertyName)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(className, "className");
            Check.NotEmpty(propertyName, "propertyName");

            ((ModelMigration)migration).AddTransformation(new RemovePropertyTransformation(className, propertyName));
        }

        public static void RenameClass(this IModelMigration migration, string oldClassName, string newClassName)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(oldClassName, "oldClassName");
            Check.NotEmpty(newClassName, "newClassName");

            ((ModelMigration)migration).AddTransformation(new RenameClassTransformation(oldClassName, newClassName));
        }

        public static void RenameProperty(this IModelMigration migration, string className, string oldPropertyName, string newPropertyName)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(className, "className");
            Check.NotEmpty(oldPropertyName, "oldPropertyName");
            Check.NotEmpty(newPropertyName, "newPropertyName");

            ((ModelMigration)migration).AddTransformation(new RenamePropertyTransformation(className, oldPropertyName, newPropertyName));
        }

        //TODO: odstranit ci nechat complex typy?
        public static void ExtractComplexType(this IModelMigration migration, string className, string complexTypeName, string[] propertiesToExtract)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(className, "className");
            Check.NotEmpty(complexTypeName, "complexTypeName");
            Check.NotNullOrEmpty(propertiesToExtract, "propertiesToExtract");

            ((ModelMigration)migration).AddTransformation(new ExtractComplexTypeTransformation(className, complexTypeName, propertiesToExtract, new NavigationPropertyCodeModel(complexTypeName)));
        }

        public static void JoinComplexType(this IModelMigration migration, string complexTypeName, string className)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(complexTypeName, "complexTypeName");
            Check.NotEmpty(className, "className");

            ((ModelMigration)migration).AddTransformation(new JoinComplexTypeTransformation(complexTypeName, className));
        }

        public static void ExtractClass(this IModelMigration migration, 
            string fromClassName, 
            string[] propertiesToExtract, 
            Func<ClassModelBuilder, ClassModel> newClassAction, 
            Func<IPrimitivePropertyBuilder, object> primaryKeysAction = null, 
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> fromClassNavigationPropAction = null,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> newClassNavigationPropAction = null,
            string[] foreignKeyColumns = null)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(fromClassName, "fromClassName");
            Check.NotNullOrEmpty(propertiesToExtract, "propertiesToExtract");
            Check.NotNull(newClassAction, "newClassAction");

            var newClassModel = newClassAction(new ClassModelBuilder());
            NavigationPropertyCodeModel fromNavigationProp = fromClassNavigationPropAction != null ? fromClassNavigationPropAction(new OneNavigationPropertyBuilder(newClassModel.Name)) : null;
            NavigationPropertyCodeModel newNavigationProp = newClassNavigationPropAction != null ? newClassNavigationPropAction(new OneNavigationPropertyBuilder(fromClassName)) : null;

            IEnumerable<PrimitivePropertyCodeModel> primaryKeys = primaryKeysAction != null ? ConvertObjectToPrimitivePropertyModel(primaryKeysAction(new PrimitivePropertyBuilder())) : null;

            ((ModelMigration)migration).AddTransformation(
                new ExtractClassTransformation(fromClassName, propertiesToExtract, newClassModel, primaryKeys, fromNavigationProp, newNavigationProp, foreignKeyColumns)
            );
        }

        public static void MergeClasses(this IModelMigration migration, 
            string principal,
            string principalNavigationProperty,
            string dependent,
            string dependentNavigationProperty,
            string[] propertiesToMerge)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(principal, "principal");
            Check.NotEmpty(dependent, "dependent");
            Check.NotNullOrEmpty(propertiesToMerge, "propertiesToMerge");

            ((ModelMigration)migration).AddTransformation(
                new MergeClassesTransformation(new SimpleAssociationEnd(principal, principalNavigationProperty), new SimpleAssociationEnd(dependent, dependentNavigationProperty), propertiesToMerge)
            );
        }

        //TODO: odstranit pokud se nerozhodnu pouzit - kdyby ne tak odstranit i ModelTransformationBuilders.cs
        //Associations
        //public static AssociationBuilder Association(this IModelMigration migration)
        //{
        //    return new AssociationBuilder((ModelMigration)migration);
        //}

        //1:1 PK
        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration,
            string principal,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            bool bothRequired = false,
            bool? willCascadeOnDelete = null)
        {
            Check.NotNull(migration, "migration");           
            Check.NotEmpty(principal, "principal");
            Check.NotNull(principalNavigationProperty, "principalNavigationProperty");
            Check.NotEmpty(dependent, "dependent");
            Check.NotNull(dependentNavigationProperty, "dependentNavigationProperty");

            var principalMultiplicity = bothRequired ? RelationshipMultiplicity.One : RelationshipMultiplicity.ZeroOrOne;

            ((ModelMigration)migration).AddTransformation(new AddOneToOnePrimaryKeyAssociationTransformation(
                new AssociationEnd(principal, principalMultiplicity, principalNavigationProperty(new OneNavigationPropertyBuilder(dependent))),
                new AssociationEnd(dependent, RelationshipMultiplicity.One, dependentNavigationProperty(new OneNavigationPropertyBuilder(principal))),
                willCascadeOnDelete));

        }
        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration,
            string principal,
            string dependent,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            bool bothRequired = false,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOnePrimaryKeyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, bothRequired, willCascadeOnDelete);
        }
        public static void AddOneToOnePrimaryKeyAssociation(this IModelMigration migration,
            string principal,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            bool bothRequired = false,
            bool? willCascadeOnDelete = null)
        {
            AddOneToOnePrimaryKeyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, bothRequired, willCascadeOnDelete);
        }

        //1:1 FK
        public static void AddOneToOneForeignKeyAssociation(this IModelMigration migration,
            string principal,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames = null,
            bool principalRequired = false,
            bool dependentRequired = true,
            bool? willCascadeOnDelete = null)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(principal, "principal");
            Check.NotNull(principalNavigationProperty, "principalNavigationProperty");
            Check.NotEmpty(dependent, "dependent");
            Check.NotNull(dependentNavigationProperty, "dependentNavigationProperty");

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
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
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
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
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
            Func<IManyNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            string[] dependentFkNames = null,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(principal, "principal");
            Check.NotNull(principalNavigationProperty, "principalNavigationProperty");
            Check.NotEmpty(dependent, "dependent");
            Check.NotNull(dependentNavigationProperty, "dependentNavigationProperty");

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
            Func<IManyNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
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
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
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
            Func<IManyNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            Func<IForeignKeyPropertyBuilder, TProps> dependentFkPropertiesAction,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(principal, "principal");
            Check.NotNull(principalNavigationProperty, "principalNavigationProperty");
            Check.NotEmpty(dependent, "dependent");
            Check.NotNull(dependentNavigationProperty, "dependentNavigationProperty");
            Check.NotNull(dependentFkPropertiesAction, "dependentFkPropertiesAction");

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
            Func<IManyNavigationPropertyBuilder, NavigationPropertyCodeModel> principalNavigationProperty,
            string dependent,
            Func<IForeignKeyPropertyBuilder, TProps> dependentFkPropertiesAction,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            AddOneToManyAssociation(migration, principal, principalNavigationProperty, dependent, _ => null, dependentFkPropertiesAction, principalRequired, willCascadeOnDelete, foreignKeyIndex);
        }
        public static void AddOneToManyAssociation<TProps>(this IModelMigration migration,
            string principal,
            string dependent,
            Func<IOneNavigationPropertyBuilder, NavigationPropertyCodeModel> dependentNavigationProperty,
            Func<IForeignKeyPropertyBuilder, TProps> dependentFkPropertiesAction,
            bool principalRequired = true,
            bool? willCascadeOnDelete = null,
            IndexAttribute foreignKeyIndex = null)
        {
            AddOneToManyAssociation(migration, principal, _ => null, dependent, dependentNavigationProperty, dependentFkPropertiesAction, principalRequired, willCascadeOnDelete, foreignKeyIndex);
        }

        //M:N
        public static void AddManyToManyAssociation(this IModelMigration migration,
            string source,
            Func<IManyNavigationPropertyBuilder, NavigationPropertyCodeModel> sourceNavigationProperty,
            string target,
            Func<IManyNavigationPropertyBuilder, NavigationPropertyCodeModel> targetNavigationProperty,
            ManyToManyJoinTable joinTable = null)
        {
            Check.NotNull(migration, "migration");
            Check.NotEmpty(source, "source");
            Check.NotNull(sourceNavigationProperty, "sourceNavigationProperty");
            Check.NotEmpty(target, "target");
            Check.NotNull(targetNavigationProperty, "targetNavigationProperty");

            ((ModelMigration)migration).AddTransformation(new AddManyToManyAssociationTransformation(
                new AssociationEnd(source, RelationshipMultiplicity.Many, sourceNavigationProperty(new ManyNavigationPropertyBuilder(target))),
                new AssociationEnd(target, RelationshipMultiplicity.Many, targetNavigationProperty(new ManyNavigationPropertyBuilder(source))),
                joinTable));
        }
        public static void AddManyToManyAssociation(this IModelMigration migration,
            string source,
            string target,
            Func<IManyNavigationPropertyBuilder, NavigationPropertyCodeModel> targetNavigationProperty,
            ManyToManyJoinTable joinTable = null)
        {
            AddManyToManyAssociation(migration, source, _ => null, target, targetNavigationProperty, joinTable);
        }
        public static void AddManyToManyAssociation(this IModelMigration migration,
            string source,
            Func<IManyNavigationPropertyBuilder, NavigationPropertyCodeModel> sourceNavigationProperty,
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
            Check.NotNull(migration, "migration");
            Check.NotEmpty(source, "source");
            Check.NotEmpty(target, "target");

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
            Check.NotNull(migration, "migration");
            Check.NotEmpty(source, "source");
            Check.NotEmpty(target, "target");

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
            Check.NotNull(migration, "migration");
            Check.NotEmpty(source, "source");
            Check.NotEmpty(target, "target");

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
            Check.NotNull(migration, "migration");
            Check.NotEmpty(source, "source");
            Check.NotEmpty(target, "target");

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
                    throw new ModelMigrationsException(Strings.PropertyDefinitionExtractionFailed); 

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
                    throw new ModelMigrationsException(Strings.ForeignKeyDefinitionExtractionFailed); 

                if (string.IsNullOrWhiteSpace(fkProperty.Name))
                {
                    fkProperty.Name = property.Name;
                }

                yield return fkProperty;
            }
        }
    }



}
