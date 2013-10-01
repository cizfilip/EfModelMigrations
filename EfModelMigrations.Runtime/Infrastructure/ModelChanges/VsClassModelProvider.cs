using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Runtime.Properties;
using EnvDTE;
using EnvDTE80;
using System;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsClassModelProvider : IClassModelProvider
    {
        private Project modelProject;
        private string modelNamespace;
        private CodeClassFinder classFinder;

        public VsClassModelProvider(Project modelProject, string modelNamespace)
        {
            this.modelProject = modelProject;
            this.modelNamespace = modelNamespace;
            this.classFinder = new CodeClassFinder(modelProject, modelNamespace);
        }

        public ClassCodeModel GetClassCodeModel(string className)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(className);

            return new VsCodeClassToClassCodeModelMapper().MapToClassCodeModel(codeClass);
        }


        
    }
}
