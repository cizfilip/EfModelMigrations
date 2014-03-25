using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Runtime.Properties;
using EnvDTE;
using EnvDTE80;
using System;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers;
using System.Collections.Generic;
using EfModelMigrations.Configuration;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsClassModelProvider : IClassModelProvider
    {
        private Project modelProject;
        private ModelMigrationsConfigurationBase configuration;
        private CodeClassFinder classFinder;
        private VsCodeClassToClassCodeModelMapper mapper;

        public VsClassModelProvider(Project modelProject, ModelMigrationsConfigurationBase configuration)
        {
            this.modelProject = modelProject;
            this.configuration = configuration;
            this.classFinder = new CodeClassFinder(modelProject);
            this.mapper = new VsCodeClassToClassCodeModelMapper(configuration.GeneratorDefaults);
        }

        public ClassCodeModel GetClassCodeModel(string className)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(configuration.ModelNamespace, className);

            return mapper.MapToClassCodeModel(codeClass);
        }
        
    }
}
