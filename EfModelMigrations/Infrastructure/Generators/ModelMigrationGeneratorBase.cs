using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public abstract class ModelMigrationGeneratorBase : IModelMigrationGenerator
    {
        public abstract GeneratedModelMigration GenerateMigration(string migrationId, string migrationDirectory, IEnumerable<ModelTransformation> transformations, string @namespace, string className);

        protected virtual IEnumerable<string> GetImportNamespaces()
        {
            return GetDefaultImportNamespaces();
        }

        protected virtual IEnumerable<string> GetDefaultImportNamespaces()
        {
            var namespaces = new List<string>();

            namespaces.Add("System");
            namespaces.Add("EfModelMigrations");
            namespaces.Add("EfModelMigrations.Infrastructure.CodeModel");
            namespaces.Add("System.ComponentModel.DataAnnotations.Schema");

            return namespaces;
        }


    }
}
