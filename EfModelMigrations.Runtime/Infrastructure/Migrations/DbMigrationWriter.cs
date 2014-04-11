using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using EfModelMigrations.Runtime.Extensions;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.IO.Compression;
using System.Xml.Linq;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    //TODO: zlepsit writer - EF writer dela trochu vic, ale hlavne bychom chteli aby se vytvorili vsechny soubory nebo zadny
    internal class DbMigrationWriter
    {
        private readonly Project project;

        public DbMigrationWriter(Project project)
        {
            this.project = project;
        }

        public void RemoveMigration(ScaffoldedMigration scaffoldedMigration)
        {
            string userCodePath;
            string resourcesPath;
            string designerCodePath;
            GetPaths(scaffoldedMigration, out userCodePath, out resourcesPath, out designerCodePath);

            var projectRoot = project.GetProjectDir();

            string[] pathsToRemove = new string[] { 
                Path.Combine(projectRoot, userCodePath), 
                Path.Combine(projectRoot, resourcesPath), 
                Path.Combine(projectRoot, designerCodePath)
            };

            foreach (var path in pathsToRemove)
            {
                var projectItem = project.DTE.Solution.FindProjectItem(path);
                if (projectItem != null)
                    projectItem.Delete();
            }
        }

        public void Write(ScaffoldedMigration scaffoldedMigration, string edmxModel)
        {
            Check.NotNull(scaffoldedMigration, "scaffoldedMigration");
            Check.NotEmpty(edmxModel, "edmxModel");

            string userCodePath;
            string resourcesPath;
            string designerCodePath;
            GetPaths(scaffoldedMigration, out userCodePath, out resourcesPath, out designerCodePath);

            project.AddContentToProject(userCodePath, scaffoldedMigration.UserCode);
            
            //use provided model as target
            scaffoldedMigration.Resources["Target"] = CompressAndEncodeEdmxModel(edmxModel);

            WriteResources(Path.Combine(project.GetProjectDir(), resourcesPath), scaffoldedMigration.Resources);
            project.AddContentToProject(designerCodePath, scaffoldedMigration.DesignerCode);

            //return userCodePath;
        }

        private void GetPaths(ScaffoldedMigration scaffoldedMigration, out string userCodePath, out string resourcesPath, out string designerCodePath)
        {
            var userCodeFileName = scaffoldedMigration.MigrationId + "." + scaffoldedMigration.Language;
            userCodePath = Path.Combine(scaffoldedMigration.Directory, userCodeFileName);
            var designerCodeFileName = scaffoldedMigration.MigrationId + ".Designer." + scaffoldedMigration.Language;
            designerCodePath = Path.Combine(scaffoldedMigration.Directory, designerCodeFileName);
            var resourcesFileName = scaffoldedMigration.MigrationId + ".resx";
            resourcesPath = Path.Combine(scaffoldedMigration.Directory, resourcesFileName);
        }

        private void WriteResources(string resourcesPath, IDictionary<string, object> resources)
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


        private string CompressAndEncodeEdmxModel(string model)
        {
            var compressed = CompressEdmxModel(model);
            return Convert.ToBase64String(compressed);
        }

        private byte[] CompressEdmxModel(string model)
        {
            var xModel = XDocument.Parse(model);

            using (var outStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outStream, CompressionMode.Compress))
                {
                    xModel.Save(gzipStream);
                }

                return outStream.ToArray();
            }
        }
    }
}
