using EfModelMigrations.Infrastructure.Model;
using EfModelMigrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF = System.Data.Entity.Migrations.Model;

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


        public override IEnumerable<ModelChangeOperation> GetModelChangeOperations()
        {
            throw new NotImplementedException();
        }

        public override ModelTransformation Inverse()
        {
            return new RemoveClassTransformation(Name);
        }

        //TODO: prejmenovat PropertyModel - koliduje se tridou se stejnym jmenem v System.Data.Entity.Migrations.Model
        public override EF.MigrationOperation GetDbMigrationOperation()
        {
            throw new NotImplementedException();
        }
    }
}
