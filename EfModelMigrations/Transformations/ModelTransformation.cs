using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public abstract class ModelTransformation
    {
        public abstract void GetModelChangeOperations();


        public abstract ModelTransformation Inverse();
    }
}
