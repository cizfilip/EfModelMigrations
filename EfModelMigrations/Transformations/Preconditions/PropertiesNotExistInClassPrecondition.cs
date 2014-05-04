using EfModelMigrations.Infrastructure;
using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Preconditions
{
    public class PropertiesNotExistInClassPrecondition : ModelTransformationPrecondition
    {
        private string className;
        private string[] properties;

        public PropertiesNotExistInClassPrecondition(string className, string[] properties)
        {
            Check.NotEmpty(className, "className");
            Check.NotNullOrEmpty(properties, "properties");

            this.className = className;
            this.properties = properties;
        }

        public override VerificationResult Verify(IClassModelProvider modelProvider)
        {
            try
            {
                var classProperties = modelProvider.GetClassCodeModel(className)
                    .Properties
                    .Select(p => p.Name)
                    .ToList();

                var existingProperties = properties.Where(p => classProperties.Contains(p)).ToList();

                if (existingProperties.Count > 0)
                {
                    return VerificationResult.Error(Strings.Precondition_PropertiesExistsInClass(string.Join(", ", existingProperties), className));
                }
            }
            catch (Exception) { }

            return VerificationResult.Successful();
        }
    }
}
