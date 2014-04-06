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
            return BuildScalarProperty(PrimitiveTypeKind.Binary, true, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Boolean(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Boolean, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Byte(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Byte, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel DateTime(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.DateTime, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel DateTimeOffset(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.DateTimeOffset, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Decimal(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Decimal, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Double(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Double, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Geography(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Geography, true, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Geometry(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Geometry, true, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Guid(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Guid, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Short(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Int16, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Int(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Int32, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Long(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Int64, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Single(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Single, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel String(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.String, true, visibility, isVirtual, isSetterPrivate);
        }

        public ScalarPropertyCodeModel Time(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return BuildScalarProperty(PrimitiveTypeKind.Time, isNullable, visibility, isVirtual, isSetterPrivate);
        }

        public EnumPropertyCodeModel Enum(
            string enumType,
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null
            )
        {
            return new EnumPropertyCodeModel(enumType, isNullable)
            {
                Visibility = visibility,
                IsVirtual = isVirtual,
                IsSetterPrivate = isSetterPrivate
            };
        }


        private ScalarPropertyCodeModel BuildScalarProperty(PrimitiveTypeKind type,
            bool isTypeNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            var scalarProperty = new ScalarPropertyCodeModel(type, isTypeNullable);

            scalarProperty.Visibility = visibility;
            scalarProperty.IsVirtual = isVirtual;
            scalarProperty.IsSetterPrivate = isSetterPrivate;

            return scalarProperty;
        }


    }

    public abstract class PrimitiveMappingBuilder
    {
        protected PrimitivePropertyCodeModel property;
        internal PrimitivePropertyCodeModel Property
        {
            get
            {
                return property;
            }
        }

        public PrimitiveMappingBuilder(PrimitivePropertyCodeModel property)
        {
            this.property = property;
        }
    }

    public class EnumMappingBuilder : PrimitiveMappingBuilder
    {
        public EnumMappingBuilder(PrimitivePropertyCodeModel property)
            :base(property)
        {
        }

        public PrimitiveMappingBuilder WithColumnMapping()
        {

            return this;
        }
    }
}
