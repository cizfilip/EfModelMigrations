﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class AddMappingInformationOperation : MappingInformationOperation
    {
        public AddMappingInformationOperation(IMappingInformation mappingInformation)
            : base(mappingInformation) { }
    }
}
