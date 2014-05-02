using EfModelMigrations.Infrastructure;
using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Preconditions
{
    public class ClassDoesNotHaveBasePrecondition : ModelTransformationPrecondition
    {
        private string className;

        public ClassDoesNotHaveBasePrecondition(string className)
        {
            Check.NotEmpty(className, "className");

            this.className = className;
        }

        public override VerificationResult Verify(IClassModelProvider modelProvider)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(modelProvider.GetClassCodeModel(className).BaseType))
                {
                    return VerificationResult.Error(Strings.Precondition_ClassHasBase(className));
                }
            }
            catch (Exception) { }

            return VerificationResult.Successful();
        }
    }
}
