using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public sealed class ScalarPropertyCodeModel : PrimitivePropertyCodeModel
    {
        public PrimitiveTypeKind Type { get; private set; }

        public ScalarPropertyCodeModel(string name, PrimitiveTypeKind type, bool isTypeNullable)
            : base(name)
        {
            this.Type = type;
            this.IsTypeNullable = EnsureCorrectNullability(isTypeNullable);
        }

        internal ScalarPropertyCodeModel(PrimitiveTypeKind type, bool isTypeNullable)
            :this(null, type, isTypeNullable)
        {
        }

        protected override PrimitivePropertyCodeModel CreateForMerge(PropertyCodeModel property, bool? newNullability = null)
        {
            bool nullability = newNullability.HasValue ? EnsureCorrectNullability(newNullability.Value) : this.IsTypeNullable;

            return new ScalarPropertyCodeModel(property.Name, Type, nullability);
        }
        
        private bool EnsureCorrectNullability(bool proposed)
        {
            if (!proposed && NullablePrimitiveTypes.Contains(Type))
                return true;

            return proposed;
        }

        public static bool TryParse(string name, string type, out ScalarPropertyCodeModel parsedProperty)
        {
            Check.NotEmpty(name, "name");
            Check.NotEmpty(type, "type");

            string unwrappedType;
            bool isNullable = PrimitivePropertyCodeModel.TryUnwrapNullability(type, out unwrappedType);

            PrimitiveTypeKind primitiveType;
            if (primitiveTypes.TryGetValue(unwrappedType, out primitiveType))
            {
                parsedProperty = new ScalarPropertyCodeModel(name, primitiveType, isNullable);
                return true;
            }

            parsedProperty = null;
            return false;
        }

        public static readonly HashSet<PrimitiveTypeKind> NullablePrimitiveTypes = new HashSet<PrimitiveTypeKind>()
        {
            PrimitiveTypeKind.String,
            PrimitiveTypeKind.Binary,
            PrimitiveTypeKind.Geography,
            PrimitiveTypeKind.GeographyCollection,
            PrimitiveTypeKind.GeographyLineString,
            PrimitiveTypeKind.GeographyMultiLineString,
            PrimitiveTypeKind.GeographyMultiPoint,
            PrimitiveTypeKind.GeographyMultiPolygon,
            PrimitiveTypeKind.GeographyPoint,
            PrimitiveTypeKind.GeographyPolygon,
            PrimitiveTypeKind.Geometry,
            PrimitiveTypeKind.GeometryCollection,
            PrimitiveTypeKind.GeometryLineString,
            PrimitiveTypeKind.GeometryMultiLineString,
            PrimitiveTypeKind.GeometryMultiPoint,
            PrimitiveTypeKind.GeometryMultiPolygon,
            PrimitiveTypeKind.GeometryPoint,
            PrimitiveTypeKind.GeometryPolygon
        };

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
