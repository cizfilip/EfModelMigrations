using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Properties;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers
{
    internal class CodeClassFinder
    {
        private Project modelProject;
        private string modelNamespace;

        public CodeClassFinder(Project modelProject, string modelNamespace)
        {
            this.modelProject = modelProject;
            this.modelNamespace = modelNamespace;
        }


        public CodeClass2 FindCodeClass(string className)
        {
            CodeClass2 codeClass = FindClassFromFullName(GetFullNameOfClass(className));

            if (codeClass == null)
                throw new ModelMigrationsException(string.Format(Resources.CannotFindClassInModelProject, className));

            return codeClass;
        }

        private string GetFullNameOfClass(string className)
        {
            return modelNamespace + "." + className;
        }


        /// <summary>
        /// Gets the type only if typeName is its fully-qualified name and it's local to the specified project
        /// </summary>
        private CodeClass2 FindClassFromFullName(string className)
        {
            try
            {
                var fullNameResult = modelProject.CodeModel.CodeTypeFromFullName(className);
                if ((fullNameResult != null) && (fullNameResult.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject))
                    return fullNameResult as CodeClass2;
            }
            catch (ArgumentException)
            {
                // For VB projects it throws an ArgumentException if the type wasn't found, whereas
                // for C# projects it returns null in that case.
            }
            return null;
        }
    }
}
