using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Preconditions
{
    public class ClassNotExistsInModelPrecondition : ModelTransformationPrecondition
    {
        private string className;

        public ClassNotExistsInModelPrecondition(string className)
        {
            Check.NotEmpty(className, "className");

            this.className = className;
        }

        public override VerificationResult Verify(IClassModelProvider modelProvider)
        {
            if (modelProvider.IsClassInModel(className))
            {
                return VerificationResult.Error(Strings.Precondition_ClassExists(className));
            }

            return VerificationResult.Successful();
        }
    }
}
