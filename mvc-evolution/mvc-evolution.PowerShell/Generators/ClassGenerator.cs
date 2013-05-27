using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mvc_evolution.PowerShell.Model;

namespace mvc_evolution.PowerShell.Generators
{
    class ClassGenerator
    {
        public string GenerateClass(ClassModel model)
        {
            StringBuilder classContent = new StringBuilder();
            
            classContent.AppendLine("using System;");
            

            classContent.AppendLine();
            classContent.AppendFormat("namespace {0}", model.Namespace);
            classContent.AppendLine();
            classContent.AppendLine("{");
            classContent.AppendFormat("\tpublic class {0}", model.Name);
            classContent.AppendLine();
            classContent.AppendLine("\t{");

            foreach (var prop in model.Properties)
            {
                classContent.Append("\t\t");
                classContent.AppendLine(GenerateProperty(prop));
            }

            classContent.AppendLine("\t}");
            classContent.AppendLine("}");


            return classContent.ToString();
        }

        public string GenerateProperty(PropertyModel model)
        {
            return string.Format("public {0} {1} {{ get; set; }}", model.Type, model.Name);
        }
    }
}
