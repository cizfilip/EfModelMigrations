using EfModelMigrations.Commands;
using EfModelMigrations.Infrastructure.CodeModel;
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
        public ClassCodeModel MapToClassCodeModel(CodeClass2 codeClass)
        {
            return new ClassCodeModel(codeClass.Namespace.FullName,
                codeClass.Name,
                MapVisibility(codeClass.Access),
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
            if (!ScalarProperty.TryParseScalar(property.Type.AsString, out scalar))
            {
                return null;
            }

            scalar.Visibility = MapVisibility(property.Access);
            scalar.IsSetterPrivate = property.Setter.Access == vsCMAccess.vsCMAccessPrivate;
            scalar.IsVirtual = property.OverrideKind == vsCMOverrideKind.vsCMOverrideKindVirtual ? true : false;

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
                    return CodeModelVisibility.Public;;
            }
        }
    }
}
