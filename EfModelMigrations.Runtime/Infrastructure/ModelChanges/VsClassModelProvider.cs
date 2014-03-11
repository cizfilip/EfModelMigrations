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

        public VsClassModelProvider(Project modelProject, ModelMigrationsConfigurationBase configuration)
        {
            this.modelProject = modelProject;
            this.configuration = configuration;
            this.classFinder = new CodeClassFinder(modelProject);
        }

        public ClassCodeModel GetClassCodeModel(string className)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(configuration.ModelNamespace, className);

            return new VsCodeClassToClassCodeModelMapper().MapToClassCodeModel(codeClass);
        }

        //TODO: pri vytvareni classcodemodelu handlovat defaultni hodnoty z configurace
        public ClassCodeModel CreateClassCodeModel(string name, 
            CodeModelVisibility? visibility, 
            string baseType, 
            IEnumerable<string> implementedInterfaces,
            IEnumerable<ScalarProperty> properties)
        {
            return new ClassCodeModel(configuration.ModelNamespace, name, visibility, baseType, implementedInterfaces, properties);
        }
    }
}
