using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.Generators.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    internal class CSharpCodeGenerator : ICodeGenerator
    {
        private IMappingInformationsGenerator mappingGenerator;

        public CSharpCodeGenerator(IMappingInformationsGenerator mappingGenerator)
        {
            this.mappingGenerator = mappingGenerator;
        }

        public string GenerateEmptyClass(string name, string @namespace, 
            CodeModelVisibility visibility, string baseType, 
            IEnumerable<string> implementedInterfaces)
        {
            return new ClassTemplate()
            {
                Name = name,
                Namespace = @namespace,
                Visibility = visibility,
                BaseType = baseType,
                ImplementedInterfaces = implementedInterfaces,
                Imports = GetDefaultImports(),
                CodeModelVisibilityMapper = CodeModelVisibilityToString
            }.TransformText();
        }

        public string GenerateProperty(PropertyCodeModel propertyModel, out string propertyName)
        {
            propertyName = propertyModel.Name;

            return new PropertyTemplate()
            {
                PropertyModel = propertyModel,
                CodeModelVisibilityMapper = CodeModelVisibilityToString
            }.TransformText();
        }

        public string GenerateDbSetProperty(string className, out string dbSetPropertyName)
        {
            dbSetPropertyName = className + "Set"; //TODO: asi spis pluralizovat jmeno
            return new DbSetPropertyTemplate()
            {
                GenericType = className,
                Name = dbSetPropertyName 
            }.TransformText();
        }

        
        public string GetFileExtensions()
        {
            return ".cs";
        }


        private IEnumerable<string> GetDefaultImports()
        {
            yield return "System";
            yield return "System.Collections.Generic";
            yield return "System.Linq";
        }

        private string CodeModelVisibilityToString(CodeModelVisibility visibility)
        {
            switch (visibility)
            {
                case CodeModelVisibility.Public:
                    return "public";
                case CodeModelVisibility.Private:
                    return "private";
                case CodeModelVisibility.Protected:
                    return "protected";
                case CodeModelVisibility.Internal:
                    return "internal";
                case CodeModelVisibility.ProtectedInternal:
                    return "protected internal";
                default:
                    return "public";
            }
        }

        public IMappingInformationsGenerator MappingGenerator
        {
            get
            {
                return mappingGenerator;
            }
        }
    }
}
