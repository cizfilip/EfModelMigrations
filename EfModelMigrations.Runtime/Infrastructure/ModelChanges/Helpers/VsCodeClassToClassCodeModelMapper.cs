using EfModelMigrations.Commands;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.Generators;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers
{
    internal class VsCodeClassToClassCodeModelMapper
    {
        private CodeGeneratorDefaults defaults;

        public VsCodeClassToClassCodeModelMapper(CodeGeneratorDefaults defaults)
        {
            this.defaults = defaults;
        }

        public ClassCodeModel MapToClassCodeModel(CodeClass2 codeClass)
        {
            return new ClassCodeModel(codeClass.Namespace.FullName,
                codeClass.Name,
                MapVisibility(codeClass.Access, defaults.Class.Visibility),
                MapBaseType(codeClass.Bases),
                MapImplementedInterfaces(codeClass.ImplementedInterfaces),
                MapProperties(codeClass.Children.OfType<CodeProperty2>())
                );
        }

        private IEnumerable<ScalarProperty> MapProperties(IEnumerable<CodeProperty2> codeProperties)
        {
            return codeProperties.Select(p => MapProperty(p)).Where(p => p != null).ToList();
        }

        private ScalarProperty MapProperty(CodeProperty2 property)
        {
            ScalarProperty scalar;
            if (!ScalarProperty.TryParse(property.Type.AsString, out scalar))
            {
                return null;
            }
            scalar.Name = property.Name;
            scalar.Visibility = MapVisibility(property.Access, defaults.Property.Visibility);

            var setterPrivate = property.Setter.Access == vsCMAccess.vsCMAccessPrivate;
            scalar.IsSetterPrivate = setterPrivate == defaults.Property.IsSetterPrivate ? (bool?)null : setterPrivate;
            var isVirtual = property.OverrideKind == vsCMOverrideKind.vsCMOverrideKindVirtual ? true : false;
            scalar.IsVirtual = isVirtual == defaults.Property.IsVirtual ? (bool?)null : isVirtual;

            return scalar;
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

        private CodeModelVisibility? MapVisibility(vsCMAccess access, CodeModelVisibility defaultVisibility)
        {
            CodeModelVisibility visibility;

            switch (access)
            {
                case vsCMAccess.vsCMAccessDefault:
                    visibility = CodeModelVisibility.Public;
                    break;
                case vsCMAccess.vsCMAccessPrivate:
                    visibility = CodeModelVisibility.Private;
                    break;
                case vsCMAccess.vsCMAccessProject:
                    visibility = CodeModelVisibility.Internal;
                    break;
                case vsCMAccess.vsCMAccessProjectOrProtected:
                    visibility = CodeModelVisibility.ProtectedInternal;
                    break;
                case vsCMAccess.vsCMAccessProtected:
                    visibility = CodeModelVisibility.Protected;
                    break;
                case vsCMAccess.vsCMAccessPublic:
                    visibility = CodeModelVisibility.Public;
                    break;
                //TODO: Co s temahle dvema??
                case vsCMAccess.vsCMAccessWithEvents:
                case vsCMAccess.vsCMAccessAssemblyOrFamily:
                default:
                    visibility = CodeModelVisibility.Public;
                    break;
            }

            if(visibility == defaultVisibility)
            { 
                return null;
            }
            else
            {
                return visibility;
            }
        }
    }
}
