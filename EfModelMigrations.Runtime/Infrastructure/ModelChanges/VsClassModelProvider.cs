using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
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
using EfModelMigrations.Runtime.Resources;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsClassModelProvider : IClassModelProvider
    {
        private Project modelProject;
        private ModelMigrationsConfigurationBase configuration;
        private CodeClassFinder classFinder;
        private VsCodeClassToClassCodeModelMapper mapper;
        private EfModel efModel;

        public EfModel EfModel
        {
            get { return efModel; }
        }

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
                var entityType = efModel.Metadata.GetEntityTypeForClass(className);
                var storeEntitySet = efModel.GetStoreEntitySetForClass(className);
                return mapper.MapToClassCodeModel(codeClass, entityType, storeEntitySet);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(Strings.CannotFindClassInModelProject(className), e);
            }
        }

        public bool IsEnumInModel(string enumName)
        {
            try
            {
                var enumCode = classFinder.FindCodeEnum(configuration.ModelNamespace, enumName);
                if(enumCode != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
