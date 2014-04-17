using EfModelMigrations.Infrastructure;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Transformations.Preconditions
{
    public class ClassExistsInModelPrecondition : ModelTransformationPrecondition
    {
        private string className;

        public ClassExistsInModelPrecondition(string className)
        {
            Check.NotEmpty(className, "className");

            this.className = className;
        }

        public override VerificationResult Verify(IClassModelProvider modelProvider)
        {
            if (!modelProvider.IsClassInModel(className))
            {
                return VerificationResult.Error(Strings.Precondition_ClassNotExists(className));
            }

            return VerificationResult.Successful();
        }
    }
}
