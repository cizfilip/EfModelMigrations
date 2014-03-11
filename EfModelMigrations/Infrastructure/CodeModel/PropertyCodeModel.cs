using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class PropertyCodeModel<PropertyType> : PropertyCodeModelBase where PropertyType : CodeModelType 
    {
        public PropertyCodeModel(PropertyType type)
            : base()
        {
            this.Type = type;
        }

        public PropertyCodeModel(string name, PropertyType type)
            : this(type)
        {
            this.Name = name;
        }

        public PropertyType Type { get; private set; }

        public override CodeModelType GetType()
        {
            return Type;
        }
    }
}
