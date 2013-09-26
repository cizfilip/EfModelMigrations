using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class RemoveClassTransformation : ModelTransformation
    {
        public string Name { get; private set; }

        public RemoveClassTransformation(string name)
        {
            this.Name = name;
        }

        public override void GetModelChangeOperations()
        {
            throw new NotImplementedException();
        }

        public override ModelTransformation Inverse()
        {
            throw new NotImplementedException();
        }
    }
}
