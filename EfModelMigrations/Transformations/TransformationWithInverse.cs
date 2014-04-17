using EfModelMigrations.Exceptions;
using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public abstract class TransformationWithInverse : ModelTransformation
    {
        protected ModelTransformation inverse;

        public TransformationWithInverse(ModelTransformation inverse)
        {
            this.inverse = inverse;
        }

        public override ModelTransformation Inverse()
        {
            if(inverse == null)
            {
                throw new InvalidOperationException(Strings.TransformationInverseMissing);
            }
            return inverse;
        }
    }
}
