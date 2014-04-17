using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public sealed class GeneratedFluetApiCall
    {
        public string TargetType { get; private set; }
        public string Content { get; private set; }

        public GeneratedFluetApiCall(string targetType, string content)
        {
            Check.NotEmpty(targetType, "targetType");
            Check.NotEmpty(content, "content");

            this.TargetType = targetType;
            this.Content = content;
        }
    }

}
