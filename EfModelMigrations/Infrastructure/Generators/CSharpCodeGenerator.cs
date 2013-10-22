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
        public string GenerateEmptyClass(ClassCodeModel classModel)
        {
            return new ClassTemplate()
            {
                ClassModel = classModel,
                Imports = GetDefaultImports(),
                CodeModelVisibilityMapper = CodeModelVisibilityToString
            }.TransformText();
        }

        public string GenerateProperty(PropertyCodeModel propertyModel)
        {
            return new PropertyTemplate()
            {
                PropertyModel = propertyModel,
                CodeModelVisibilityMapper = CodeModelVisibilityToString
            }.TransformText();
        }

        public string GenerateDbSetProperty(ClassCodeModel classForAddProperty)
        {
            //TODO: Takhle jde delat dpset property bez public setteru
            //public DbSet<Blog> Blogs
            //{
            //    get { return Set<Blog>(); }
            //}

            //TODO: Pluralizovat jmeno pomoci EF
            //private IPluralizationService _pluralizationService
            //= DbConfiguration.DependencyResolver.GetService<IPluralizationService>();

            return new DbSetPropertyTemplate()
            {
                GenericType = classForAddProperty.Name,
                Name = classForAddProperty.Name + "Set" //TODO: asi spis pluralizovat jmeno
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


        
    }
}
