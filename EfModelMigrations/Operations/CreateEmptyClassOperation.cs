using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EfModelMigrations.Operations
{
    public class CreateEmptyClassOperation : IModelChangeOperation
    {
        public string Name { get; private set; }
        public CodeModelVisibility? Visibility { get; private set; }


        public CreateEmptyClassOperation(string name, CodeModelVisibility? visibility = null)
        {
            Check.NotEmpty(name, "name");

            this.Name = name;
            this.Visibility = visibility;
        }
    }
}
