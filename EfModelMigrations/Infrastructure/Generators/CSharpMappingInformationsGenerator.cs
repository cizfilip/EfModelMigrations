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
        private static readonly string ModelBuilderParameterName = "modelBuilder";

        public string GetPrefixForOnModelCreatingUse(string entityName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ModelBuilderParameterName)
                .Append(".Entity<")
                .Append(entityName)
                .Append(">()");
            AppendIndentedNewLine(sb);
            sb.Append(".");
            return sb.ToString();
        }

        public GeneratedFluetApiCall GenerateFluentApiCall(EfFluentApiCallChain callChain)
        {
            var generatedMethodCalls = callChain.FluentApiCalls.Select(m => GenerateOneFluentApiCall(m));
            var result = string.Join(GetMethodCallSeparator(), generatedMethodCalls) + ";";

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
                .Append(" => ");

            if(parameter.PropertyNames.Length == 1)
            {
                sb.Append(lambdaParameterName)
                    .Append(".")
                    .Append(parameter.PropertyNames[0]);
            }
            else
            {
                sb.Append("new { ");
                foreach (var property in parameter.PropertyNames)
                {
                    sb.Append(lambdaParameterName)
                        .Append(".")
                        .Append(parameter)
                        .Append(", ");
                }
                sb.Length = sb.Length - 2;
                sb.Append(" }");
            }
        }

        protected virtual void GenerateParameter(ValueParameter parameter, StringBuilder sb)
        {
            sb.Append(parameter.Value.ToString());
        }

        protected virtual void GenerateParameter(StringParameter parameter, StringBuilder sb)
        {
            sb.Append("\"")
                .Append(parameter.Value)
                .Append("\"");
        }

        protected virtual void GenerateParameter(MapMethodParameter parameter, StringBuilder sb)
        {
            var lambdaParameterName = GetLambdaParameterNameForMapMethod();
            sb.Append(lambdaParameterName)
                .Append(" => ");

            int mapMethodCallsCount = parameter.MapCalls.Count;

            if(mapMethodCallsCount == 0)
            {
                sb.Append("{ }");
            }
            else if (mapMethodCallsCount == 1)
            {
                sb.Append(lambdaParameterName)
                    .Append(".")
                    .Append(GenerateOneFluentApiCall(parameter.MapCalls[0]));
            }
            else
            {
                AppendIndentedNewLine(sb);
                sb.Append("{");
                foreach (var mapMethodCall in parameter.MapCalls)
                {
                    AppendIndentedNewLine(sb, 2);
                    sb.Append(lambdaParameterName)
                        .Append(".")
                        .Append(GenerateOneFluentApiCall(mapMethodCall))
                        .Append(";");
                }
                AppendIndentedNewLine(sb);
                sb.Append("}");
            }
        }


        protected virtual string GetLambdaParameterName(string className)
        {
            return className.Take(1).Single().ToString().ToLower();
        }

        protected virtual string GetLambdaParameterNameForMapMethod()
        {
            return "m";
        }

        private string GetMethodCallSeparator()
        {
            return new StringBuilder().AppendLine().Append(Indent).Append(".").ToString();
        }

        private void AppendIndentedNewLine(StringBuilder sb, int indent = 1)
        {
            sb.AppendLine();
            for (int i = 0; i < indent; i++)
            {
                sb.Append(Indent);
            }
        }

        protected virtual string GenerateMethodName(EfFluentApiMethods method)
        {
            return Enum.GetName(typeof(EfFluentApiMethods), method);
        }        
    }
}
