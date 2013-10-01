using EfModelMigrations.Infrastructure.CodeModel;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers
{
    internal class VsCodeClassToClassCodeModelMapper
    {
        public ClassCodeModel MapToClassCodeModel(CodeClass2 codeClass)
        {
            return new ClassCodeModel()
            {
                Name = codeClass.Name,
                Namespace = codeClass.Namespace.FullName,
                BaseType = MapBaseType(codeClass.Bases),
                ImplementedInterfaces = MapImplementedInterfaces(codeClass.ImplementedInterfaces),
                Properties = MapProperties(codeClass.Children.OfType<CodeProperty2>()),
                Visibility = MapVisibility(codeClass.Access)
            };
        }

        private IEnumerable<PropertyCodeModel> MapProperties(IEnumerable<CodeProperty2> codeProperties)
        {
            return codeProperties.Select(p => MapProperty(p)).ToList();
        }

        private PropertyCodeModel MapProperty(CodeProperty2 property)
        {
            return new PropertyCodeModel()
            {
                Name = property.Name,
                Type = property.Type.AsFullName,
                Visibility = MapVisibility(property.Access),
                IsSetterPrivate = property.Setter.Access == vsCMAccess.vsCMAccessPrivate
            };
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
