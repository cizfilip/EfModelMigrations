using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators.Templates
{
    internal partial class ModelMigrationTemplate
    {
        private string methodBodyIndent = "            ";

        public IEnumerable<string> Imports { get; set; }
        public string Namespace { get; set; }
        public string MigrationId { get; set; }
        public string ClassName { get; set; }
        public string UpMethod { get; set; }
        public string DownMethod { get; set; }

        private string IndentMethodBody(string methodBody, string indent)
        {
            StringBuilder builder = new StringBuilder();

            var methodBodyLines = methodBody.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < methodBodyLines.Length; i++)
            {
                builder.Append(indent);
                if (i != methodBodyLines.Length - 1)
                {
                    builder.AppendLine(methodBodyLines[i]);
                }
                else
                {
                    builder.Append(methodBodyLines[i]);
                }
            }

            foreach (var line in methodBodyLines)
            {
                
            }

            

            //TODO: Na konci metod jsou 2 prazdne radky

            return builder.ToString();
        }
    }
}
