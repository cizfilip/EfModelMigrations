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

        private static readonly HashSet<EfFluentApiMethods> AssociationMethods = new HashSet<EfFluentApiMethods>()
        {
            EfFluentApiMethods.HasMany,
            EfFluentApiMethods.HasOptional,
            EfFluentApiMethods.HasRequired,
            EfFluentApiMethods.WithMany,
            EfFluentApiMethods.WithOptionalDependent,
            EfFluentApiMethods.WithOptionalPrincipal,
            EfFluentApiMethods.WithRequired,
            EfFluentApiMethods.WithRequiredDependent,
            EfFluentApiMethods.WithRequiredPrincipal,
        };
    

        public VsMappingInformationsRemover(CSharpRegexMappingGenerator regexGenerator)
        {
            this.regexGenerator = regexGenerator;
        }

        public string GetRegexPrefix(string entityName)
        {
            return regexGenerator.GetPrefixForOnModelCreatingUse(entityName);
        }

        public GeneratedFluetApiCall GetRegex(IMappingInformation mappingInfo)
        {
            try
            {
                dynamic info = mappingInfo;
                return Remove(info);
            }
            catch (RuntimeBinderException e)
            {
                //TODO: string do resourcu
                throw new ModelMigrationsException(string.Format("Cannot remove mapping information of type {0}. Remover implementation is missing.", mappingInfo.GetType().Name), e);
            }
        }

        protected virtual GeneratedFluetApiCall Remove(AssociationInfo mappingInfo)
        {
            var originalChain = mappingInfo.BuildEfFluentApiCallChain();
            originalChain.FluentApiCalls = originalChain.FluentApiCalls.Where(f => AssociationMethods.Contains(f.Method)).ToList();

            return regexGenerator.GenerateFluentApiCall(originalChain);
        }

    }
}
