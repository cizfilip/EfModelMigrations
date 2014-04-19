using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Resources;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsMappingInformationsRemover
    {
        private CSharpRegexMappingGenerator regexGenerator;

        public VsMappingInformationsRemover(CSharpRegexMappingGenerator regexGenerator)
        {
            this.regexGenerator = regexGenerator;
        }

        public string GetRegexPrefix(string entityName)
        {
            return regexGenerator.GetPrefixForOnModelCreatingUse(entityName);
        }

        public IEnumerable<GeneratedFluetApiCall> GetRegExps(IRemoveMappingInformation mappingInfo)
        {
            try
            {
                dynamic info = mappingInfo;
                return RemoveRegExps(info);
            }
            catch (RuntimeBinderException e)
            {
                throw new ModelMigrationsException(Strings.MappingInformationRemover_MissingImplementation(mappingInfo.GetType().Name), e);
            }
        }

        protected virtual IEnumerable<GeneratedFluetApiCall> RemoveRegExps(RemoveAssociationMapping mappingInfo)
        {
            if(mappingInfo.Source.HasNavigationPropertyName)
            {
                var methodChain = new EfFluentApiCallChain(mappingInfo.Source.ClassName)
                    .AddMethodCall(EfFluentApiMethods.HasMany, CreatePropertySelectorParameter(mappingInfo.Source.ClassName, mappingInfo.Source.NavigationPropertyName))
                    .AddMethodCall(EfFluentApiMethods.WithMany, CreatePropertySelectorParameter(mappingInfo.Target.ClassName, mappingInfo.Target.NavigationPropertyName));

                yield return regexGenerator.GenerateFluentApiCall(methodChain);
            }

            if (mappingInfo.Target.HasNavigationPropertyName)
            {
                var methodChain = new EfFluentApiCallChain(mappingInfo.Target.ClassName)
                    .AddMethodCall(EfFluentApiMethods.HasMany, CreatePropertySelectorParameter(mappingInfo.Target.ClassName, mappingInfo.Target.NavigationPropertyName))
                    .AddMethodCall(EfFluentApiMethods.WithMany, CreatePropertySelectorParameter(mappingInfo.Source.ClassName, mappingInfo.Source.NavigationPropertyName));

                yield return regexGenerator.GenerateFluentApiCall(methodChain);
            }
        }

        protected virtual IEnumerable<GeneratedFluetApiCall> RemoveRegExps(RemovePropertyMapping mappingInfo)
        {
            var methodChain = new EfFluentApiCallChain(mappingInfo.ClassName)
                .AddMethodCall(EfFluentApiMethods.Property, CreatePropertySelectorParameter(mappingInfo.ClassName, mappingInfo.PropertyName));

            yield return regexGenerator.GenerateFluentApiCall(methodChain);
        }

        protected virtual IEnumerable<GeneratedFluetApiCall> RemoveRegExps(RemoveClassMapping mappingInfo)
        {
            var methodChain = new EfFluentApiCallChain(mappingInfo.Name);

            yield return regexGenerator.GenerateFluentApiCall(methodChain);
        }


        private PropertySelectorParameter CreatePropertySelectorParameter(string className, string navigationProperty)
        {
            if (string.IsNullOrEmpty(navigationProperty) || string.IsNullOrEmpty(className))
            {
                return null;
            }

            return new PropertySelectorParameter(className, navigationProperty);
        }
    }
}
