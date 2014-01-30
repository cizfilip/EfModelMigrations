using EfModelMigrations.Exceptions;
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
                //TODO: string do resourcu
                throw new ModelMigrationsException("Inversion transformation was requested but not provided before!");
            }
            return inverse;
        }
    }
}
