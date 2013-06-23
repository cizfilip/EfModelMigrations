using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;
using mvc_evolution.PowerShell.Extensions;
using mvc_evolution.PowerShell.Generators;
using mvc_evolution.PowerShell.Locators;
using mvc_evolution.PowerShell.Model;
using System.Data.Entity.Migrations.Design;
using System.Reflection;

namespace mvc_evolution.PowerShell.Commands
{
    internal class CreateEntityCommand : DomainCommand
    {
        private string className;
        private IEnumerable<PropertyModel> classProperties;

        public CreateEntityCommand(string className, string[] properties) 
        {
            this.className = className;
         
            this.classProperties = from p in properties
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
            var project = Project;

            //Assembly efAssembly = EFAssembly;

            //var ctxExtensions = efAssembly.GetType("System.Data.Entity.Utilities.DbContextExtensions", throwOnError: true);

            //WriteLine(ctxExtensions.Name);


            AddClassToProject(project);

            AddOrModifyDbContextInProject(project);

            AddMigrationToProject(project);
        }

        private void AddMigrationToProject(Project project)
        {
            WriteLine("Scaffolding new migration...");

            Assembly efAssembly = EFAssembly;

            //Build project
            if (!project.TryBuild())
            {
                throw new InvalidOperationException("Unable to add new migration, project failed to build!");
            }
                        



            
        }

        private void AddOrModifyDbContextInProject(Project project)
        {
            WriteLine("Searching for DbContexts in project...");

            var dbContexts = DbContextLocator.FindDbContextsInProject(project, this);


            var dbContextsCount = dbContexts.Count();

            if (dbContextsCount > 1)
            {
                throw new InvalidOperationException("There is more than one DbContext in project!");
            }

            if (dbContextsCount == 0)
            {
                WriteLine("No DbContexts found.");
                AddDbContextToProject(project);
            }
            else
            {
                WriteLine("Found DbContext in project...");
                ModifyDbContextInProject(project, dbContexts.Single());
            }

        }

        private void ModifyDbContextInProject(Project project, CodeType dbContext)
        {
            WriteLine("Adding new DbSet property to DbContext: " + dbContext.Name);

            string dbSetPropertyContent = new DbContextGenerator().GenerateProperty(new DbSetPropertyModel(){
                Name = Pluralize(className),
                Type = className
            });

            dbContext.AddMemberFromSourceCode(dbSetPropertyContent);

            WriteLine("Successfully added new DbSet property to DbContext: " + dbContext.Name);
        }

        private void AddDbContextToProject(Project project)
        {
            string contextName = Regex.Replace(project.Name, "[^a-zA-Z0-9]", "") + "Context";
            string contextFileName = contextName + ".cs";
            string contextNamespace = project.GetRootNamespace();

            WriteLine("Creating DbContext file: " + contextFileName);

            var contextModel = new DbContextModel()
            {
                Name = contextName,
                Namespace = contextNamespace,
                DbSetProperties = new List<DbSetPropertyModel>() 
                    { new DbSetPropertyModel()
                        {
                            Name = Pluralize(className),
                            Type = className
                        } 
                    }
            };

            string content = new DbContextGenerator().GenerateContextClass(contextModel);

            string newFilePath = Path.Combine(project.GetProjectDir(), contextFileName);

            project.AddContentToProject(newFilePath, content);

            WriteLine("Successfully created DbContext file: " + contextFileName);
        }

        private void AddClassToProject(Project project)
        {
            var fileName = className + ".cs";

            WriteLine("Creating class file: " + fileName);

            string newFilePath = Path.Combine(project.GetProjectDir(), fileName);

            var classModel = new ClassModel()
            {
                Name = className,
                Namespace = project.GetRootNamespace(),
                Properties = classProperties
            };
            string content = new ClassGenerator().GenerateClass(classModel);

            project.AddContentToProject(newFilePath, content);

            WriteLine("Successfully created class file: " + fileName);
        }

        private string Pluralize(string str)
        {
            //TODO: Try peek to EF source code for better pluralization method 
            //hint: pluralizationService = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(new CultureInfo(Culture));
            return str + "s";
        }
    }
}
