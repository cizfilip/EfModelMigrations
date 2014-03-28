using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Operations.Mapping.Model;
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
                //TODO: string do resourcu
                throw new ModelMigrationsException(string.Format("Cannot remove mapping information of type {0}. Remover implementation is missing.", mappingInfo.GetType().Name), e);
            }
        }

        protected virtual IEnumerable<GeneratedFluetApiCall> RemoveRegExps(RemoveAssociationMapping mappingInfo)
        {
            if(!string.IsNullOrEmpty(mappingInfo.Source.NavigationPropertyName))
            {
                var methodChain = new EfFluentApiCallChain(mappingInfo.Source.ClassName)
                    .AddMethodCall(EfFluentApiMethods.HasMany, CreatePropertySelectorParameter(mappingInfo.Source.ClassName, mappingInfo.Source.NavigationPropertyName))
                    .AddMethodCall(EfFluentApiMethods.HasMany, CreatePropertySelectorParameter(mappingInfo.Target.ClassName, mappingInfo.Target.NavigationPropertyName));

                yield return regexGenerator.GenerateFluentApiCall(methodChain);
            }

            if (!string.IsNullOrEmpty(mappingInfo.Target.NavigationPropertyName))
            {
                var methodChain = new EfFluentApiCallChain(mappingInfo.Target.ClassName)
                    .AddMethodCall(EfFluentApiMethods.HasMany, CreatePropertySelectorParameter(mappingInfo.Target.ClassName, mappingInfo.Target.NavigationPropertyName))
                    .AddMethodCall(EfFluentApiMethods.HasMany, CreatePropertySelectorParameter(mappingInfo.Source.ClassName, mappingInfo.Source.NavigationPropertyName));

                yield return regexGenerator.GenerateFluentApiCall(methodChain);
            }
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
