using EfModelMigrations.Commands;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Infrastructure.EntityFramework.EdmExtensions;
using System.Data.Entity.Core.Mapping;
using EfModelMigrations.Runtime.Extensions;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Infrastructure.EntityFramework;
using System.Data.Entity.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers
{
    internal class VsCodeClassToClassCodeModelMapper
    {
        private CodeGeneratorDefaults defaults;
        private EfModel efModel;

        public VsCodeClassToClassCodeModelMapper(CodeGeneratorDefaults defaults, EfModel efModel)
        {
            this.defaults = defaults;
            this.efModel = efModel;
        }

        public ClassCodeModel MapToClassCodeModel(CodeClass2 codeClass, EntityType entityType, EntitySet storeEntitySet)
        {
            var scalarProperties = MapProperties(codeClass, entityType.Properties);
            var primaryKeys = entityType.KeyProperties.Select(k => k.Name);

            //TODO: associace obecne - metoda InitializeForeignKeyLists na EntitySet - hiearchie komplikuje nejspis i asociace!!!! - viz extnsiony v ef IsAssignableFrom a IsSubtypeOf
            //TODO: foreign keys
            //var fks = efModel.Metadata.EntityContainer.AssociationSets.Select(a => a.ElementType)
            //    .Where(at => at.IsForeignKey && at.Constraint != null && at.Constraint.ToRole.GetEntityType().Equals(entityType))
            //    .SelectMany(a => a.Constraint.ToProperties);

            return new ClassCodeModel(
                codeClass.Name,
                new TableName(storeEntitySet.Table, storeEntitySet.Schema),
                MapVisibility(codeClass.Access),
                MapBaseType(codeClass.Bases),
                MapImplementedInterfaces(codeClass.ImplementedInterfaces),
                scalarProperties,
                MapNavigationProperties(codeClass, entityType.NavigationProperties),
                scalarProperties.Where(p => primaryKeys.Contains(p.Name)).ToList()
                )
                {
                    StoreEntityType = storeEntitySet.ElementType,
                    ConceptualEntityType = entityType
                };
        }

        private IEnumerable<PrimitivePropertyCodeModel> MapProperties(CodeClass2 codeClass, IEnumerable<EdmProperty> properties)
        {
            return properties.Select(p => MapProperty(
                                                codeClass.FindProperty(p.Name), 
                                                p,
                                                efModel.GetStoreColumnForProperty(codeClass.Name, p.Name)
                                            )).ToList();
        }

        private IEnumerable<NavigationPropertyCodeModel> MapNavigationProperties(CodeClass2 codeClass, IEnumerable<NavigationProperty> properties)
        {
            return properties.Select(p => MapNavigationProperty(codeClass.FindProperty(p.Name), p)).ToList();
        }


        private PrimitivePropertyCodeModel MapProperty(CodeProperty2 codeProperty, EdmProperty edmProperty, EdmProperty column)
        {
            Check.NotNull(codeProperty, "codeProperty");

            PrimitivePropertyCodeModel property = null;

            bool isPropertyNullable = GetNullability(codeProperty);

            if(edmProperty.IsPrimitiveType)
            {
                property = new ScalarPropertyCodeModel(edmProperty.Name, edmProperty.PrimitiveType.PrimitiveTypeKind, isPropertyNullable);
            }
            else if(edmProperty.IsEnumType)
            {
                property = new EnumPropertyCodeModel(edmProperty.Name, edmProperty.EnumType.Name, isPropertyNullable);
            }
            else
            {
                throw new InvalidOperationException("Unknown type for edmProperty"); //TODO: string do resourcu
            }
            
            MapPrimitiveProperty(property, edmProperty, column);

            MapPropertyCodeModel(property, codeProperty);

            return property;
        }

        

        private NavigationPropertyCodeModel MapNavigationProperty(CodeProperty2 codeProperty, NavigationProperty edmProperty)
        {
            Check.NotNull(codeProperty, "codeProperty");

            NavigationPropertyCodeModel navigationProperty = new NavigationPropertyCodeModel(edmProperty.Name,
                edmProperty.ToEndMember.GetEntityType().Name,
                edmProperty.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many);

            MapPropertyCodeModel(navigationProperty, codeProperty);

            return navigationProperty;
        }

        private void MapPropertyCodeModel(PropertyCodeModel property, CodeProperty2 codeProperty)
        {
            property.Visibility = MapVisibilityWitDefaults(codeProperty.Access, defaults.Property.Visibility);
            var setterPrivate = codeProperty.Setter.Access == vsCMAccess.vsCMAccessPrivate;
            property.IsSetterPrivate = setterPrivate == defaults.Property.IsSetterPrivate ? (bool?)null : setterPrivate;
            var isVirtual = codeProperty.OverrideKind == vsCMOverrideKind.vsCMOverrideKindVirtual ? true : false;
            property.IsVirtual = isVirtual == defaults.Property.IsVirtual ? (bool?)null : isVirtual;
        }

        private void MapPrimitiveProperty(PrimitivePropertyCodeModel property, EdmProperty edmProperty, EdmProperty column)
        {
            var storeEntity = column.DeclaringType as EntityType;

            //property.Column.ColumnOrder = storeEntity != null ? storeEntity.Properties.Select((p, i) => Tuple.Create((int?)i, p)).Where(p => p.Item2.Name.EqualsOrdinal(column.Name)).Select(i => i.Item1).SingleOrDefault() : null;
            property.Column.ColumnAnnotations.AddRange(column.CustomAnnotationsAsDictionary());
            property.Column.ColumnName = column.Name;
            property.Column.ColumnType = column.TypeName;
            property.Column.DatabaseGeneratedOption = (DatabaseGeneratedOption)column.StoreGeneratedPattern;
            property.Column.IsConcurrencyToken = edmProperty.ConcurrencyMode == ConcurrencyMode.Fixed ? true : false;
            property.Column.IsFixedLength = edmProperty.IsFixedLength;
            property.Column.IsMaxLength = edmProperty.IsMaxLength;
            property.Column.MaxLength = edmProperty.MaxLength;
            property.Column.IsNullable = column.Nullable;
            property.Column.IsRowVersion = column.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.Binary && column.MaxLength == 8 && column.IsStoreGeneratedComputed;
            property.Column.IsUnicode = edmProperty.IsUnicode;
            property.Column.Precision = edmProperty.Precision;
            property.Column.Scale = edmProperty.Scale;

            //TODO: momentalne nemapuji PropertyName... a column order
            //efModel.Metadata.EntityContainerMapping.EntitySetMappings.SelectMany(es => es.ModificationFunctionMappings).Where(mf => mf.EntityType == edmProperty.DeclaringType).SelectMany(ef => ef.DeleteFunctionMapping.ParameterBindings.)
            //property.Column.ParameterName = edm
        }


        private IEnumerable<string> MapImplementedInterfaces(CodeElements codeElements)
        {
            List<string> interfaces = new List<string>();
            foreach (CodeElement @interface in codeElements)
            {
                interfaces.Add(@interface.FullName);
            }
            return interfaces;
        }

        private string MapBaseType(CodeElements codeElements)
        {
            if (codeElements.Count == 1)
            {
                return codeElements.Item(1).FullName;
            }
            else
            {
                return null;
            }
        }

        private CodeModelVisibility? MapVisibilityWitDefaults(vsCMAccess access, CodeModelVisibility defaultVisibility)
        {
            CodeModelVisibility visibility = MapVisibility(access);

            if (visibility == defaultVisibility)
            {
                return null;
            }
            else
            {
                return visibility;
            }
        }

        private CodeModelVisibility MapVisibility(vsCMAccess access)
        {
            switch (access)
            {
                case vsCMAccess.vsCMAccessDefault:
                    return CodeModelVisibility.Public;
                case vsCMAccess.vsCMAccessPrivate:
                    return CodeModelVisibility.Private;
                case vsCMAccess.vsCMAccessProject:
                    return CodeModelVisibility.Internal;
                case vsCMAccess.vsCMAccessProjectOrProtected:
                    return CodeModelVisibility.ProtectedInternal;
                case vsCMAccess.vsCMAccessProtected:
                    return CodeModelVisibility.Protected;
                case vsCMAccess.vsCMAccessPublic:
                    return CodeModelVisibility.Public;
                //TODO: Co s temahle dvema??
                case vsCMAccess.vsCMAccessWithEvents:
                case vsCMAccess.vsCMAccessAssemblyOrFamily:
                default:
                    return CodeModelVisibility.Public;
            }
        }

        private bool GetNullability(CodeProperty2 codeProperty)
        {
            string type = codeProperty.Type.AsString;
            string _;
            return PrimitivePropertyCodeModel.TryUnwrapNullability(type, out _);
        }
    }
}
