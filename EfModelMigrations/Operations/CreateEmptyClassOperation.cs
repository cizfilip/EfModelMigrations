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
        

        //TODO: Podporovat i nasledujici property pri vytvareni tridy
        public string Namespace { get; private set; }
        public CodeModelVisibility Visibility { get; private set; }
        public IEnumerable<string> ImplementedInterfaces { get; private set; }


        public CreateEmptyClassOperation(string name)
        {
            this.Name = name;
        }

        //public CreateClassOperation(string name, 
        //    CodeModelVisibility visibility,
        //    IEnumerable<string> implementedInterfaces)
        //{
        //    //TODO: defaults mst be supplied from configuration
        //    //TODO: throw if name, namespace or model is null !!!!!
        //    Name = name;
        //    Visibility = visibility;
        //    ImplementedInterfaces = implementedInterfaces ?? Enumerable.Empty<string>();
        //}

    }
}
