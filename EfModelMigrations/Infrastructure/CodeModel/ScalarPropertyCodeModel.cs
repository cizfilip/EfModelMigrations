using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public sealed class ScalarPropertyCodeModel : PropertyCodeModel
    {
        private ColumnModel columnModel;
        public ColumnModel ColumnModel { 
            get
            {
                return columnModel;
            }
         }

        public PrimitiveTypeKind Type
        {
            get
            {
                return columnModel.Type;
            }
        }

        public ScalarPropertyCodeModel(string name, PrimitiveTypeKind type)
            : base(name)
        {
            this.columnModel = new ColumnModel(type);
        }

        internal ScalarPropertyCodeModel(PrimitiveTypeKind type)
            :this(null, type)
        {
        }

        
        public static bool TryParse(string type, out ScalarPropertyCodeModel parsedProperty)
        {
            string lowerType = type.ToLowerInvariant();
            PrimitiveTypeKind primitiveType;

            if (primitiveTypes.TryGetValue(lowerType, out primitiveType))
            {
                parsedProperty = new ScalarPropertyCodeModel(primitiveType);
                return true;
            }

            parsedProperty = null;
            return false;
        }

        private static readonly Dictionary<string, PrimitiveTypeKind> primitiveTypes = new Dictionary<string, PrimitiveTypeKind>()
        {
            {"byte[]", PrimitiveTypeKind.Binary},
            {"system.byte[]", PrimitiveTypeKind.Binary},

            {"bool", PrimitiveTypeKind.Boolean},
            {"boolean", PrimitiveTypeKind.Boolean},
            {"system.boolean", PrimitiveTypeKind.Boolean},

            {"byte", PrimitiveTypeKind.Byte},
            {"system.byte", PrimitiveTypeKind.Byte},

            {"datetime", PrimitiveTypeKind.DateTime},
            {"system.datetime", PrimitiveTypeKind.DateTime},
            
            {"datetimeoffset", PrimitiveTypeKind.DateTimeOffset},
            {"system.datetimeoffset", PrimitiveTypeKind.DateTimeOffset},

            {"decimal", PrimitiveTypeKind.Decimal},
            {"system.decimal", PrimitiveTypeKind.Decimal},

            {"double", PrimitiveTypeKind.Double},
            {"system.double", PrimitiveTypeKind.Double},

            {"dbgeography", PrimitiveTypeKind.Geography},
            {"system.data.spatial.dbgeography", PrimitiveTypeKind.Geography},

            {"dbgeometry", PrimitiveTypeKind.Geometry},
            {"system.data.spatial.dbgeometry", PrimitiveTypeKind.Geometry},

            {"guid", PrimitiveTypeKind.Guid},
            {"system.guid", PrimitiveTypeKind.Guid},

            {"short", PrimitiveTypeKind.Int16},
            {"int16", PrimitiveTypeKind.Int16},
            {"system.int16", PrimitiveTypeKind.Int16},

            {"int", PrimitiveTypeKind.Int32},
            {"int32", PrimitiveTypeKind.Int32},
            {"system.int32", PrimitiveTypeKind.Int32},

            {"long", PrimitiveTypeKind.Int64},
            {"int64", PrimitiveTypeKind.Int64},
            {"system.int64", PrimitiveTypeKind.Int64},

            {"sbyte", PrimitiveTypeKind.SByte},
            {"system.sbyte", PrimitiveTypeKind.SByte},
            
            {"float", PrimitiveTypeKind.Single},
            {"single", PrimitiveTypeKind.Single},
            {"system.single", PrimitiveTypeKind.Single},

            {"string", PrimitiveTypeKind.String},
            {"system.string", PrimitiveTypeKind.String},

            {"timespan", PrimitiveTypeKind.Time},
            {"system.timespan", PrimitiveTypeKind.Time},
        };
    }
}
