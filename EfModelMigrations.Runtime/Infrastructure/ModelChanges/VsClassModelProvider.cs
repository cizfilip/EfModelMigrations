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
        private EfModelMetadata metadata;

        public VsClassModelProvider(Project modelProject, ModelMigrationsConfigurationBase configuration, string edmxModel)
        {
            this.modelProject = modelProject;
            this.configuration = configuration;
            this.classFinder = new CodeClassFinder(modelProject);
            this.mapper = new VsCodeClassToClassCodeModelMapper(configuration.GeneratorDefaults);
            this.metadata = EfModelMetadata.Load(edmxModel);
        }

        public ClassCodeModel GetClassCodeModel(string className)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(configuration.ModelNamespace, className);

            var entityType = metadata.EdmItemCollection.GetItems<EntityType>().SingleOrDefault(e => e.Name.EqualsOrdinal(className));
            if(entityType == null)
            {
                throw new ModelMigrationsException(string.Format("Cannot find class {0} in ef model.", className)); //TODO: string do resourcu
            }
            

            return mapper.MapToClassCodeModel(codeClass, entityType);
        }
        
    }
}
