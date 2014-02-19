using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Generators.Templates;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public class CSharpMappingInformationsGenerator : IMappingInformationsGenerator
    {
        private static readonly string Indent = "    ";
        private static readonly string ModelBuilderParameterName = "    ";

        public string GetPrefixForOnModelCreatingUse(string entityName)
        {
            return string.Format("{0}.Entity<{1}>().", ModelBuilderParameterName, entityName);
        }

        public GeneratedFluetApiCall GenerateFluentApiCall(EfFluentApiCallChain callChain)
        {
            var generatedMethodCalls = callChain.FluentApiCalls.Select(m => GenerateOneFluentApiCall(m));
            var result = string.Join(GetMethodCallSeparator(), generatedMethodCalls);

            return new GeneratedFluetApiCall()
            {
                Content = result,
                TargetType = callChain.EntityType
            };
        }

        protected virtual string GenerateOneFluentApiCall(EfFluetApiCall fluentApiCall)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GenerateMethodName(fluentApiCall.Method))
                    .Append("(");

            foreach (var param in fluentApiCall.Parameters)
            {
                GenerateParameter(param, sb);
                sb.Append(", ");
            }
            sb.Length--;

            sb.Append(")");

            return sb.ToString();
        }

        protected virtual void GenerateParameter(IEfFluentApiMethodParameter parameter, StringBuilder sb)
        {
            try
            {
                dynamic param = parameter;

                GenerateParameter(param, sb);
            }
            catch (RuntimeBinderException e)
            {
                //TODO: string do resourcu
                throw new ModelMigrationsException(string.Format("Cannot generate mapping information, because parameter of type {0} is not supported", parameter.GetType().Name), e);
            }
        }

        protected virtual void GenerateParameter(PropertySelectorParameter parameter, StringBuilder sb)
        {
            var lambdaParameterName = GetLambdaParameterName(parameter.ClassName);
            sb.Append(lambdaParameterName)
                .Append(" => ")
                .Append(lambdaParameterName)
                .Append(".")
                .Append(parameter.PropertyName);
        }

        protected virtual void GenerateParameter(ValueParameter parameter, StringBuilder sb)
        {
            sb.Append(parameter.Value.ToString());
        }


        protected virtual string GetLambdaParameterName(string className)
        {
            return className.Take(1).Single().ToString().ToLower();
        }

        private string GetMethodCallSeparator()
        {
            return new StringBuilder().AppendLine().Append(Indent).Append(".").ToString();
        }


        protected virtual string GenerateMethodName(EfFluentApiMethods method)
        {
            return Enum.GetName(typeof(EfFluentApiMethods), method);
        }


        
    }
}
