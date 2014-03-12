using EfModelMigrations.Infrastructure.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class CSharpRegexMappingGenerator : CSharpMappingInformationsGenerator
    {
        private static readonly string AllowWhiteSpaceToken = @"\s*";

        
        protected override string GetSyntaxToken(CSharpTokenType type)
        {
            if(type == CSharpTokenType.StatementSeparator)
            {
                return AllowWhiteSpaceToken + base.GetSyntaxToken(type);
            }
            else
            {
                return AllowWhiteSpaceToken + base.GetSyntaxToken(type) + AllowWhiteSpaceToken;
            }
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
