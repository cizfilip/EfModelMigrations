using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public class CreateClassCommand : ModelMigrationCommand
    {
        public CreateClassCommand(string[] parameters) : base(parameters) { }


        public override void GetTransformations()
        {
            throw new NotImplementedException();
        }
    }
}
