using EfModelMigrations.Exceptions;
using EfModelMigrations.Runtime.Resources;
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
        private Project project;

        public CodeClassFinder(Project project)
        {
            this.project = project;
        }

        public CodeEnum FindCodeEnum(string @namespace, string name)
        {
            return FindCodeEnumFromFullName(GetFullNameOfClass(@namespace, name));
        }

        public CodeEnum FindCodeEnumFromFullName(string enumFullName)
        {
            CodeEnum codeEnum = FindCodeTypeFromFullNameInternal(enumFullName) as CodeEnum;

            if (codeEnum == null)
                throw new ModelMigrationsException(Strings.CannotFindEnumInModelProject(enumFullName));

            return codeEnum;
        }


        public CodeClass2 FindCodeClass(string @namespace, string className)
        {
            return FindCodeClassFromFullName(GetFullNameOfClass(@namespace, className));
        }

        public CodeClass2 FindCodeClassFromFullName(string classFullName)
        {
            CodeClass2 codeClass = FindCodeTypeFromFullNameInternal(classFullName) as CodeClass2;

            if (codeClass == null)
                throw new ModelMigrationsException(Strings.CannotFindClassInModelProject(classFullName));

            return codeClass;
        }

        private string GetFullNameOfClass(string @namespace, string className)
        {
            return @namespace + "." + className;
        }


        /// <summary>
        /// Gets the type only if typeName is its fully-qualified name and it's local to the specified project
        /// </summary>
        private CodeType FindCodeTypeFromFullNameInternal(string fullName)
        {
            try
            {
                var fullNameResult = project.CodeModel.CodeTypeFromFullName(fullName);
                if ((fullNameResult != null) && (fullNameResult.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject))
                    return fullNameResult;
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
