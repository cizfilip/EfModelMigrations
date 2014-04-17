using EfModelMigrations.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Preconditions
{
    public abstract class ModelTransformationPrecondition
    {
        public abstract VerificationResult Verify(IClassModelProvider modelProvider);
    }
}
