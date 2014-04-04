using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    //TODO: neni metoda pro Sbyte - je to s nim nejake podivne ani ColumnBuilder v EF ji nema - jeste kouknout o co jde
    public sealed class PrimitivePropertyBuilder : IFluentInterface
    {
        public ScalarPropertyCodeModel Binary(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Binary, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Boolean(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Boolean, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Byte(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Byte, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel DateTime(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.DateTime, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel DateTimeOffset(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.DateTimeOffset, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Decimal(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Decimal, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Double(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Double, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Geography(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Geography, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Geometry(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Geometry, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Guid(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Guid, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Short(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Int16, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Int(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Int32, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Long(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Int64, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Single(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Single, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel String(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.String, visibility, isVirtual, isSetterPrivate);
        }
        public ScalarPropertyCodeModel Time(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Time, visibility, isVirtual, isSetterPrivate);
        }

        public EnumPropertyCodeModel Enum(
            string enumType,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null
            )
        {
            return new EnumPropertyCodeModel(enumType)
            {
                Visibility = visibility,
                IsVirtual = isVirtual,
                IsSetterPrivate = isSetterPrivate
            };
        }


        private ScalarPropertyCodeModel BuildScalarProperty(PrimitiveTypeKind type,
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
