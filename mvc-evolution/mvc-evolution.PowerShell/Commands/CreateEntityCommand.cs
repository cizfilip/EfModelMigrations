using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using mvc_evolution.PowerShell.Extensions;

namespace mvc_evolution.PowerShell.Commands
{
    internal class CreateEntityCommand : DomainCommand
    {
        private string className;

        public CreateEntityCommand(string className) 
        {
            this.className = className;

            Execute();
        }

        protected override void ExecuteCore()
        {
            

            var fileName = className + ".cs";

            WriteLine(fileName);

            var project = Project;
            
            string newFilePath = Path.Combine(project.GetProjectDir(), fileName);

            WriteLine(newFilePath);
            
            string content = GetNewClassContent();

            WriteLine(content);

            File.WriteAllText(newFilePath, content);

            project.ProjectItems.AddFromFile(newFilePath);

        }

        private string GetNewClassContent()
        {
            StringBuilder classContent = new StringBuilder();

            classContent.AppendFormat("namespace dummy.{0}.dumy", className);
            classContent.AppendLine("{");
            classContent.AppendFormat("\tclass {0} ", className);
            classContent.AppendLine("\t{");


            classContent.AppendLine("\t}");
            classContent.AppendLine("}");


            return classContent.ToString();
        }


    }
}
