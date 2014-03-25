using EfModelMigrations.Commands;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Runtime.Extensions;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using Edm = System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers
{
    //TODO: resit enumy!!!
    internal class VsCodeClassToClassCodeModelMapper
    {
        private CodeGeneratorDefaults defaults;

        public VsCodeClassToClassCodeModelMapper(CodeGeneratorDefaults defaults)
        {
            this.defaults = defaults;
        }

        public ClassCodeModel MapToClassCodeModel(CodeClass2 codeClass, Edm.EntityType entityType)
        {
            var scalarProperties = MapScalarProperties(codeClass, entityType.Properties);
            var primaryKeys = entityType.KeyProperties.Select(k => k.Name);

            return new ClassCodeModel(
                codeClass.Name,
                MapVisibility(codeClass.Access),
                MapBaseType(codeClass.Bases),
                MapImplementedInterfaces(codeClass.ImplementedInterfaces),
                scalarProperties,
                MapNavigationProperties(codeClass, entityType.NavigationProperties),
                scalarProperties.Where(p => primaryKeys.Contains(p.Name)).ToList()
                );
        }

        private IEnumerable<ScalarProperty> MapScalarProperties(CodeClass2 codeClass, IEnumerable<Edm.EdmProperty> properties)
        {
            //map only properies of primitive type -> no enum is mapped
            return properties.Where(p => p.IsPrimitiveType).Select(p => MapScalarProperty(codeClass.FindProperty(p.Name), p)).Where(p => p != null).ToList();
        }

        private IEnumerable<NavigationProperty> MapNavigationProperties(CodeClass2 codeClass, IEnumerable<Edm.NavigationProperty> properties)
        {
            return properties.Select(p => MapNavigationProperty(codeClass.FindProperty(p.Name), p)).Where(p => p != null).ToList();
        }


        private ScalarProperty MapScalarProperty(CodeProperty2 codeProperty, Edm.EdmProperty edmProperty)
        {
            Check.NotNull(codeProperty, "codeProperty");

            ScalarProperty scalarProperty = new ScalarProperty(edmProperty.Name, edmProperty.PrimitiveType.PrimitiveTypeKind);

            MapProperty(scalarProperty, codeProperty);
            return scalarProperty;
        }

        private NavigationProperty MapNavigationProperty(CodeProperty2 codeProperty, Edm.NavigationProperty edmProperty)
        {
            Check.NotNull(codeProperty, "codeProperty");

            NavigationProperty navigationProperty = new NavigationProperty(edmProperty.Name,
                edmProperty.ToEndMember.GetEntityType().Name,
                edmProperty.ToEndMember.RelationshipMultiplicity == Edm.RelationshipMultiplicity.Many);           

            MapProperty(navigationProperty, codeProperty);

            return navigationProperty;
        }

        private void MapProperty(PropertyCodeModel property, CodeProperty2 codeProperty)
        {
            property.Visibility = MapVisibilityWitDefaults(codeProperty.Access, defaults.Property.Visibility);
            var setterPrivate = codeProperty.Setter.Access == vsCMAccess.vsCMAccessPrivate;
            property.IsSetterPrivate = setterPrivate == defaults.Property.IsSetterPrivate ? (bool?)null : setterPrivate;
            var isVirtual = codeProperty.OverrideKind == vsCMOverrideKind.vsCMOverrideKindVirtual ? true : false;
            property.IsVirtual = isVirtual == defaults.Property.IsVirtual ? (bool?)null : isVirtual;
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
    }
}
