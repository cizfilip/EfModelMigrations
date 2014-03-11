using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    //TODO: neni metoda pro Sbyte - je to s nim nejake podivne ani ColumnBuilder v EF ji nema - jeste kouknout o co jde
    public class ScalarPropertyBuilder : IFluentInterface
    {
        public ScalarProperty Binary( 
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Binary, visibility, isVirtual);
        }

        public ScalarProperty Boolean(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Boolean, visibility, isVirtual);
        }

        public ScalarProperty Byte(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Byte, visibility, isVirtual);
        }

        public ScalarProperty DateTime(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.DateTime, visibility, isVirtual);
        }

        public ScalarProperty DateTimeOffset(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.DateTimeOffset, visibility, isVirtual);
        }

        public ScalarProperty Decimal(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Decimal, visibility, isVirtual);
        }

        public ScalarProperty Double(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Double, visibility, isVirtual);
        }

        public ScalarProperty Geography(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Geography, visibility, isVirtual);
        }

        public ScalarProperty Geometry(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Geometry, visibility, isVirtual);
        }

        public ScalarProperty Guid(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Guid, visibility, isVirtual);
        }

        public ScalarProperty Short(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int16, visibility, isVirtual);
        }

        public ScalarProperty Int(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int32, visibility, isVirtual);
        }

        public ScalarProperty Long(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int64, visibility, isVirtual);
        }

        public ScalarProperty Single(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Single, visibility, isVirtual);
        }

        public ScalarProperty String(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.String, visibility, isVirtual);
        }
        public ScalarProperty Time(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            return BuildProperty(PrimitiveTypeKind.Time, visibility, isVirtual);
        }



        private ScalarProperty BuildProperty(PrimitiveTypeKind type,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null)
        {
            var scalarProperty = new ScalarProperty(new ScalarType(type));
            
            if(visibility.HasValue)
            {
                scalarProperty.Visibility = visibility.Value;
            }
            if (isVirtual.HasValue)
            {
                scalarProperty.IsVirtual = isVirtual.Value;
            }

            return scalarProperty;
        }
    }
}
