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
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    //TODO: zlepsit writer - EF writer dela trochu vic
    internal class DbMigrationWriter
    {
        private readonly Project project;

        public DbMigrationWriter(Project project)
        {
            this.project = project;
        }

        public void Write(ScaffoldedMigration scaffoldedMigration, string edmxModel, HistoryTracker historyTracker)
        {
            Check.NotNull(scaffoldedMigration, "scaffoldedMigration");
            Check.NotEmpty(edmxModel, "edmxModel");

            string userCodePath;
            string resourcesPath;
            string designerCodePath;
            GetPaths(scaffoldedMigration, out userCodePath, out resourcesPath, out designerCodePath);

            historyTracker.MarkItemAdded(userCodePath);
            historyTracker.MarkItemAdded(resourcesPath);
            historyTracker.MarkItemAdded(designerCodePath);

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
