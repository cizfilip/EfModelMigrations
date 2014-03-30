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
using EfModelMigrations.Infrastructure.EntityFramework;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using EfModelMigrations.Extensions;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsClassModelProvider : IClassModelProvider
    {
        private Project modelProject;
        private ModelMigrationsConfigurationBase configuration;
        private CodeClassFinder classFinder;
        private VsCodeClassToClassCodeModelMapper mapper;
        private EfModel efModel;

        public VsClassModelProvider(Project modelProject, ModelMigrationsConfigurationBase configuration, string edmxModel)
        {
            this.modelProject = modelProject;
            this.configuration = configuration;
            this.classFinder = new CodeClassFinder(modelProject);
            this.efModel = new EfModel(edmxModel);
            this.mapper = new VsCodeClassToClassCodeModelMapper(configuration.GeneratorDefaults, efModel);
        }

        public ClassCodeModel GetClassCodeModel(string className)
        {
            Check.NotEmpty(className, "className");

            CodeClass2 codeClass = classFinder.FindCodeClass(configuration.ModelNamespace, className);

            try
            {
                var entityType = efModel.GetEntityTypeForClass(className);
                return mapper.MapToClassCodeModel(codeClass, entityType);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.CannotFindClassInModelProject, className), e);
            }
        }
        
    }
}
