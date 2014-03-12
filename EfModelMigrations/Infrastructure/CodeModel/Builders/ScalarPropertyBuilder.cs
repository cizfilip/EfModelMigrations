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
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Binary, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Boolean(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Boolean, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Byte(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Byte, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty DateTime(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.DateTime, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty DateTimeOffset(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.DateTimeOffset, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Decimal(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Decimal, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Double(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Double, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Geography(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Geography, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Geometry(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Geometry, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Guid(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Guid, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Short(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int16, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Int(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int32, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Long(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int64, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty Single(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Single, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarProperty String(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.String, visibility, isVirtual, isSetterPrivate);
        }
        public ScalarProperty Time(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Time, visibility, isVirtual, isSetterPrivate);
        }



        private ScalarProperty BuildProperty(PrimitiveTypeKind type,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            var scalarProperty = new ScalarProperty(type);

            scalarProperty.Visibility = visibility;
            scalarProperty.IsVirtual = isVirtual;
            scalarProperty.IsSetterPrivate = isSetterPrivate;

            return scalarProperty;
        }
    }
}
