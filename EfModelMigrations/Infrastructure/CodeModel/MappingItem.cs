using EfModelMigrations.Operations.Mapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class MappingItem<TValue>
    {
        public TValue Value { get; private set; }

        public MappingItem(TValue value)
        {
            this.Value = value;
        }

        public abstract EfFluetApiCall AsEfFluentApiCall();
        
    }
}
