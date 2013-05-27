using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mvc_evolution.PowerShell.Model;

namespace mvc_evolution.PowerShell.Generators
{
    class DbContextGenerator
    {
        public string GenerateContextClass(DbContextModel model)
        {
            StringBuilder classContent = new StringBuilder();

            classContent.AppendLine("using System;");
            classContent.AppendLine("using System.Collections.Generic;");
            classContent.AppendLine("using System.Data.Entity;");
            classContent.AppendLine("using System.Linq;");

            classContent.AppendLine();

            classContent.AppendFormat("namespace {0}", model.Namespace);
            classContent.AppendLine();
            classContent.AppendLine("{");
            classContent.AppendFormat("\tpublic class {0} : DbContext", model.Name);
            classContent.AppendLine();
            classContent.AppendLine("\t{");

            foreach (var prop in model.DbSetProperties)
            {
                classContent.Append("\t\t");
                classContent.AppendLine(GenerateProperty(prop));
            }

            classContent.AppendLine("\t}");
            classContent.AppendLine("}");


            return classContent.ToString();
        }

        public string GenerateProperty(DbSetPropertyModel model)
        {
            return string.Format("public DbSet<{0}> {1} {{ get; set; }}", model.Type, model.Name);
        }
    }
}

