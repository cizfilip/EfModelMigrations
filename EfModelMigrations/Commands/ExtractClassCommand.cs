using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Resources;
using EfModelMigrations.Transformations;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public class ExtractClassCommand : ModelMigrationsCommand
    {
        private string newClassName;
        private string fromClassName;
        private string[] propertiesToExtract;

        public ExtractClassCommand(string newClassName, string fromClassName, string[] propertiesToExtract)
        {
            //TODO: strings to resources
            if (string.IsNullOrWhiteSpace(newClassName))
            {
                throw new ModelMigrationsException("Name of extracted class missing.");
            }
            if (string.IsNullOrWhiteSpace(fromClassName))
            {
                throw new ModelMigrationsException("Name of class from extract missing.");
            }
            if (propertiesToExtract == null || propertiesToExtract.Length == 0)
            {
                throw new ModelMigrationsException("No properties to extract.");
            }

            this.newClassName = newClassName;
            this.fromClassName = fromClassName;
            this.propertiesToExtract = propertiesToExtract;
        }

        public override IEnumerable<ModelTransformation> GetTransformations(IClassModelProvider modelProvider)
        {
            yield return new ExtractClassTransformation(fromClassName, propertiesToExtract, new ClassModel(newClassName));
        }

        protected override string GetDefaultMigrationName()
        {
            return "ExtractClass" + newClassName;
        }
    }
}
