using EfModelMigrations.Infrastructure;
using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Preconditions
{
    public class PropertiesExistsInClassPrecondition : ModelTransformationPrecondition
    {
        private string className;
        private string[] properties;

        public PropertiesExistsInClassPrecondition(string className, string[] properties)
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

                var nonExistingProperties = properties.Where(p => !classProperties.Contains(p)).ToList(); 

                if (nonExistingProperties.Count > 0)
                {
                    return VerificationResult.Error(Strings.Precondition_PropertiesNotExistsInClass(string.Join(", ", nonExistingProperties), className));
                }
            }
            catch (Exception) { }

            return VerificationResult.Successful();
        }
    }
}
