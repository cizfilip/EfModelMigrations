using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public abstract class ModelMigrationCommand
    {
        public ModelMigrationCommand(string[] parameters)
        {
            this.Parameters = parameters;
        }


        public string[] Parameters { get; private set; }

        public abstract void GetTransformations();
    }
}
