using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel.Builders
{
    public sealed class PrimitivePropertyBuilder : IFluentInterface
    {
        public BinaryMappingBuilder Binary(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new BinaryMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Binary, true, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Boolean(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Boolean, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Byte(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Byte, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public DateTimeMappingBuilder DateTime(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new DateTimeMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.DateTime, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public DateTimeMappingBuilder DateTimeOffset(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new DateTimeMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.DateTimeOffset, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public DecimalMappingBuilder Decimal(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new DecimalMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Decimal, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Double(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Double, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Geography(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Geography, true, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Geometry(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Geometry, true, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Guid(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Guid, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Short(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Int16, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Int(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Int32, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Long(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Int64, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Single(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Single, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public StringMappingBuilder String(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new StringMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.String, true, visibility, isVirtual, isSetterPrivate)
            );
        }

        public DateTimeMappingBuilder Time(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new DateTimeMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Time, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public PrimitiveMappingBuilder Enum(
            string enumType,
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null
            )
        {
            return new PrimitiveMappingBuilder(
                new EnumPropertyCodeModel(enumType, isNullable)
                {
                    Visibility = visibility,
                    IsVirtual = isVirtual,
                    IsSetterPrivate = isSetterPrivate
                }
            );
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

    public class PrimitiveMappingBuilder : IFluentInterface
    {
        private PrimitivePropertyCodeModel property;
        internal PrimitivePropertyCodeModel Property
        {
            get
            {
                return property;
            }
        }

        internal PrimitiveMappingBuilder(PrimitivePropertyCodeModel property)
        {
            this.property = property;
        }

        public PrimitiveMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            MapPrimitive(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);

            return this;
        }

        public PrimitiveMappingBuilder WithIndex(IndexAttribute index)
        {
            property.Column.ColumnAnnotations.Add(IndexAnnotation.AnnotationName, new IndexAnnotation(index));
            return this;
        }

        public PrimitiveMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            property.Column.DatabaseGeneratedOption = option;
            return this;
        }


        protected void MapPrimitive(string columnName = null, 
            bool? isNullable = null,
            string databaseType = null, 
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            property.Column.ColumnName = columnName;
            property.Column.IsNullable = isNullable;
            property.Column.ColumnType = databaseType;
            property.Column.ColumnOrder = order;
            property.Column.ParameterName = parameterName;
            property.Column.IsConcurrencyToken = isConcurrencyToken;
        }
    }

    public abstract class LengthMappingBuilder : PrimitiveMappingBuilder
    {
        public LengthMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }

        public LengthMappingBuilder WithLengthOptions(int? maxLength = null, bool? isMaxLength = null, bool? isFixedLegth = null)
        {
            Property.Column.MaxLength = maxLength;
            Property.Column.IsMaxLength = isMaxLength;
            Property.Column.IsFixedLength = isFixedLegth;

            return this;
        }
    }

    public class StringMappingBuilder : LengthMappingBuilder
    {
        public StringMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }
        public new StringMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            base.WithColumnMapping(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);
            return this;
        }

        public new StringMappingBuilder WithIndex(IndexAttribute index)
        {
            base.WithIndex(index);
            return this;
        }

        public new StringMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            base.HasDatabaseGeneratedOption(option);
            return this;
        }

        public new StringMappingBuilder WithLengthOptions(int? maxLength = null, bool? isMaxLength = null, bool? isFixedLegth = null)
        {
            base.WithLengthOptions(maxLength, isMaxLength, isFixedLegth);
            return this;
        }

        public StringMappingBuilder IsUnicode(bool value)
        {
            Property.Column.IsUnicode = value;
            return this;
        }
    }

    public class BinaryMappingBuilder : LengthMappingBuilder
    {
        public BinaryMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }
        public new BinaryMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            base.WithColumnMapping(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);
            return this;
        }

        public new BinaryMappingBuilder WithIndex(IndexAttribute index)
        {
            base.WithIndex(index);
            return this;
        }

        public new BinaryMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            base.HasDatabaseGeneratedOption(option);
            return this;
        }

        public new BinaryMappingBuilder WithLengthOptions(int? maxLength = null, bool? isMaxLength = null, bool? isFixedLegth = null)
        {
            base.WithLengthOptions(maxLength, isMaxLength, isFixedLegth);
            return this;
        }

        public BinaryMappingBuilder IsTimespan()
        {
            Property.Column.IsRowVersion = true;
            return this;
        }
    }

    public class DateTimeMappingBuilder : PrimitiveMappingBuilder
    {
        public DateTimeMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }

        public new DateTimeMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            base.WithColumnMapping(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);
            return this;
        }

        public new DateTimeMappingBuilder WithIndex(IndexAttribute index)
        {
            base.WithIndex(index);
            return this;
        }

        public new DateTimeMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            base.HasDatabaseGeneratedOption(option);
            return this;
        }

        public DateTimeMappingBuilder WithPrecision(byte precision)
        {
            Property.Column.Precision = precision;
            return this;
        }
    }

    public class DecimalMappingBuilder : PrimitiveMappingBuilder
    {
        public DecimalMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }
        public new DecimalMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            base.WithColumnMapping(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);
            return this;
        }

        public new DecimalMappingBuilder WithIndex(IndexAttribute index)
        {
            base.WithIndex(index);
            return this;
        }

        public new DecimalMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            base.HasDatabaseGeneratedOption(option);
            return this;
        }

        public DecimalMappingBuilder WithPrecision(byte precision, byte scale)
        {
            Property.Column.Precision = precision;
            Property.Column.Scale = scale;
            return this;
        }
    }
}
