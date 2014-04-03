using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    //TODO: neni metoda pro Sbyte - je to s nim nejake podivne ani ColumnBuilder v EF ji nema - jeste kouknout o co jde
    public sealed class ScalarPropertyBuilder : IFluentInterface
    {
        public ScalarPropertyCodeModel Binary(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Binary, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Boolean(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Boolean, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Byte(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Byte, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel DateTime(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.DateTime, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel DateTimeOffset(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.DateTimeOffset, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Decimal(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Decimal, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Double(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Double, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Geography(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Geography, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Geometry(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Geometry, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Guid(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Guid, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Short(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int16, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Int(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int32, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Long(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Int64, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Single(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Single, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel String(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.String, visibility, isVirtual, isSetterPrivate);
        }
        public ScalarPropertyCodeModel Time(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildProperty(PrimitiveTypeKind.Time, visibility, isVirtual, isSetterPrivate);
        }



        private ScalarPropertyCodeModel BuildProperty(PrimitiveTypeKind type,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            var scalarProperty = new ScalarPropertyCodeModel(type);

            scalarProperty.Visibility = visibility;
            scalarProperty.IsVirtual = isVirtual;
            scalarProperty.IsSetterPrivate = isSetterPrivate;

            return scalarProperty;
        }
    }
}
