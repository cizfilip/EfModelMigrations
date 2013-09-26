using EfModelMigrations.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations
{
    public class CreateClassTransformation : ModelTransformation
    {
        public string Name { get; private set; }
        public IEnumerable<PropertyModel> Properties { get; private set; }
        
        public CreateClassTransformation(string name, IEnumerable<PropertyModel> properties)
        {
            this.Name = name;
            this.Properties = properties;
        }
        

        public override void GetModelChangeOperations()
        {
            throw new NotImplementedException();
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(Name);
        }
    }
}
