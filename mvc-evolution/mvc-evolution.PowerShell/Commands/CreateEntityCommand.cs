using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using mvc_evolution.PowerShell.Extensions;
using mvc_evolution.PowerShell.Model;

namespace mvc_evolution.PowerShell.Commands
{
    internal class CreateEntityCommand : DomainCommand
    {
        private string className;
        private IEnumerable<PropertyModel> properties;

        public CreateEntityCommand(string className, string[] properties) 
        {
            this.className = className;
         
            this.properties = from p in properties
                              let splitted = p.Split(':')
                              select new PropertyModel()
                              {
                                  Name = splitted.FirstOrDefault(),
                                  Type = splitted.LastOrDefault()
                              };

            Execute();
        }

        protected override void ExecuteCore()
        {
            var fileName = className + ".cs";

            WriteLine(fileName);

            var project = Project;
            
            string newFilePath = Path.Combine(project.GetProjectDir(), fileName);

            WriteLine(newFilePath);

            var classModel = new ClassModel()
            {
                Name = className,
                Namespace = project.GetRootNamespace(),
                Properties = properties
            };

            string content = GetNewClassContent(classModel);

            WriteLine(content);

            File.WriteAllText(newFilePath, content);

            project.ProjectItems.AddFromFile(newFilePath);

        }

        private string GetNewClassContent(ClassModel classModel)
        {
            StringBuilder classContent = new StringBuilder();

            classContent.AppendFormat("namespace {0}", classModel.Namespace);
            classContent.AppendLine();
            classContent.AppendLine("{");
            classContent.AppendFormat("\tpublic class {0} ", classModel.Name);
            classContent.AppendLine();
            classContent.AppendLine("\t{");

            foreach (var prop in classModel.Properties)
            {
                classContent.AppendFormat("\t\tpublic {0} {1} {{ get; set; }}", prop.Type, prop.Name);
                classContent.AppendLine();
            }

            classContent.AppendLine("\t}");
            classContent.AppendLine("}");


            return classContent.ToString();
        }


    }
}
