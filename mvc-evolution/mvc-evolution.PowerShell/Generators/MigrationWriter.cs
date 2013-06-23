using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using mvc_evolution.PowerShell.Extensions;

namespace mvc_evolution.PowerShell.Generators
{
    internal class MigrationWriter
    {
        private readonly Project project;

        public MigrationWriter(Project project)
        {
            this.project = project;
        }

        public string Write(ScaffoldedMigration scaffoldedMigration)
        {
            var projectRoot = project.GetProjectDir();

            var userCodeFileName = scaffoldedMigration.MigrationId + "." + scaffoldedMigration.Language;
            var userCodePath = Path.Combine(projectRoot, scaffoldedMigration.Directory, userCodeFileName);
            var designerCodeFileName = scaffoldedMigration.MigrationId + ".Designer." + scaffoldedMigration.Language;
            var designerCodePath = Path.Combine(projectRoot, scaffoldedMigration.Directory, designerCodeFileName);
            var resourcesFileName = scaffoldedMigration.MigrationId + ".resx";
            var resourcesPath = Path.Combine(projectRoot, scaffoldedMigration.Directory, resourcesFileName);


            project.AddContentToProject(userCodePath, scaffoldedMigration.UserCode);
            

            WriteResources(userCodePath, resourcesPath, scaffoldedMigration.Resources);
            project.AddContentToProject(designerCodePath, scaffoldedMigration.DesignerCode);

            return userCodePath;
        }

        private void WriteResources(string userCodePath, string resourcesPath, IDictionary<string, object> resources)
        {
            using (var writer = new ResXResourceWriter(resourcesPath))
            {
                foreach (var res in resources)
                {
                    writer.AddResource(res.Key, res.Value);
                }
            }


            project.AddFileToProject(resourcesPath);
        }
    }
}
