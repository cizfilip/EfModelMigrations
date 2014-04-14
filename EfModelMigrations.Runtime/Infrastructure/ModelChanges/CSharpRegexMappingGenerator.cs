using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Operations.Mapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{

    //TODO: kromě povolování whitespace okolo CSharpTokenType bych taky mel povolovat komentáře typu... /**/
    //@"[\s/*]*";
    internal class CSharpRegexMappingGenerator : CSharpMappingInformationsGenerator
    {
        private static readonly string AllowWhiteSpaceToken = @"\s*";
        private static readonly string AnyLambdaParameterName = @"[a-zA-Z0-9]+";
        private static readonly string AnyLetter = @"[a-zA-Z]+";
        

        private static readonly HashSet<EfFluentApiMethods> HasAssociationMethods = new HashSet<EfFluentApiMethods>()
        {
            EfFluentApiMethods.HasMany,
            EfFluentApiMethods.HasOptional,
            EfFluentApiMethods.HasRequired,
        };

        private static readonly HashSet<EfFluentApiMethods> WithAssociationMethods = new HashSet<EfFluentApiMethods>()
        {
            EfFluentApiMethods.WithMany,
            EfFluentApiMethods.WithOptionalDependent,
            EfFluentApiMethods.WithOptionalPrincipal,
            EfFluentApiMethods.WithRequired,
            EfFluentApiMethods.WithRequiredDependent,
            EfFluentApiMethods.WithRequiredPrincipal,
        };

        
        protected override string GetSyntaxToken(CSharpTokenType type)
        {
            if(type == CSharpTokenType.StatementSeparator)
            {
                return ".*" + Regex.Escape(base.GetSyntaxToken(type));
            }
            else
            {
                return AllowWhiteSpaceToken + Regex.Escape(base.GetSyntaxToken(type)) + AllowWhiteSpaceToken;
            }
        }

        protected override string GenerateMethodName(EfFluentApiMethods method)
        {
            if(HasAssociationMethods.Contains(method))
            {
                return string.Format("Has" + AnyLetter);
            }
            else if(WithAssociationMethods.Contains(method))
            {
                return string.Format("With" + AnyLetter);
            }
            else
            {
                return base.GenerateMethodName(method);
            }
        }

        protected override string GetLambdaParameterName(string className)
        {
            return AnyLambdaParameterName;
        }

        protected override string GetLambdaParameterNameForMapMethod()
        {
            return AnyLambdaParameterName;
        }

        protected override void AppendIndentedNewLine(StringBuilder sb, int indent = 1)
        {
            return; //noop during regex generation
        }

        protected override string GetWhiteSpace()
        {
            return "";
        }
    }
}
