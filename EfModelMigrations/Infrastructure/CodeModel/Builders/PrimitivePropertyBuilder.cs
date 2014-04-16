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
    public interface IPrimitivePropertyBuilder : IFluentInterface
    {
        IBinaryMappingBuilder Binary(CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Boolean(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Byte(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IDateTimeMappingBuilder DateTime(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IDateTimeMappingBuilder DateTimeOffset(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IDecimalMappingBuilder Decimal(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Double(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Geography(CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Geometry(CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Guid(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Short(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Int(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Long(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Single(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IStringMappingBuilder String(CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IDateTimeMappingBuilder Time(bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
        IPrimitiveMappingBuilder Enum(string enumType, bool isNullable = false, CodeModelVisibility? visibility = null, bool? isVirtual = null, bool? isSetterPrivate = null);
    }

    public sealed class PrimitivePropertyBuilder : IPrimitivePropertyBuilder
    {
        public IBinaryMappingBuilder Binary(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new BinaryMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Binary, true, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Boolean(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Boolean, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Byte(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Byte, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IDateTimeMappingBuilder DateTime(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new DateTimeMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.DateTime, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IDateTimeMappingBuilder DateTimeOffset(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new DateTimeMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.DateTimeOffset, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IDecimalMappingBuilder Decimal(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new DecimalMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Decimal, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Double(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Double, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Geography(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Geography, true, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Geometry(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Geometry, true, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Guid(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Guid, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Short(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Int16, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Int(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Int32, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Long(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Int64, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Single(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new PrimitiveMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Single, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IStringMappingBuilder String(
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new StringMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.String, true, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IDateTimeMappingBuilder Time(
            bool isNullable = false,
            CodeModelVisibility? visibility = null,
            bool? isVirtual = null,
            bool? isSetterPrivate = null)
        {
            return new DateTimeMappingBuilder(
                BuildScalarProperty(PrimitiveTypeKind.Time, isNullable, visibility, isVirtual, isSetterPrivate)
            );
        }

        public IPrimitiveMappingBuilder Enum(
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


    public interface IPrimitiveMappingBuilder : IFluentInterface
    {
        IPrimitiveMappingBuilder WithColumnMapping(string columnName = null, bool? isNullable = null, string databaseType = null, int? order = null, string parameterName = null, bool? isConcurrencyToken = null);
        IPrimitiveMappingBuilder WithIndex(IndexAttribute index);
        IPrimitiveMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option);
    }

    public class PrimitiveMappingBuilder : IPrimitiveMappingBuilder
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

        public IPrimitiveMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            MapPrimitive(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);

            return this;
        }

        public IPrimitiveMappingBuilder WithIndex(IndexAttribute index)
        {
            property.Column.ColumnAnnotations.Add(IndexAnnotation.AnnotationName, new IndexAnnotation(index));
            return this;
        }

        public IPrimitiveMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
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

    public interface IStringMappingBuilder : IFluentInterface
    {
        IStringMappingBuilder WithColumnMapping(string columnName = null, bool? isNullable = null, string databaseType = null, int? order = null, string parameterName = null, bool? isConcurrencyToken = null);
        IStringMappingBuilder WithIndex(IndexAttribute index);
        IStringMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option);
        IStringMappingBuilder WithLengthOptions(int? maxLength = null, bool? isMaxLength = null, bool? isFixedLegth = null);
        IStringMappingBuilder IsUnicode(bool value);
    }

    public class StringMappingBuilder : LengthMappingBuilder, IStringMappingBuilder
    {
        public StringMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }
        public new IStringMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            base.WithColumnMapping(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);
            return this;
        }

        public new IStringMappingBuilder WithIndex(IndexAttribute index)
        {
            base.WithIndex(index);
            return this;
        }

        public new IStringMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            base.HasDatabaseGeneratedOption(option);
            return this;
        }

        public new IStringMappingBuilder WithLengthOptions(int? maxLength = null, bool? isMaxLength = null, bool? isFixedLegth = null)
        {
            base.WithLengthOptions(maxLength, isMaxLength, isFixedLegth);
            return this;
        }

        public IStringMappingBuilder IsUnicode(bool value)
        {
            Property.Column.IsUnicode = value;
            return this;
        }
    }

    public interface IBinaryMappingBuilder : IFluentInterface
    {
        IBinaryMappingBuilder WithColumnMapping(string columnName = null, bool? isNullable = null, string databaseType = null, int? order = null, string parameterName = null, bool? isConcurrencyToken = null);
        IBinaryMappingBuilder WithIndex(IndexAttribute index);
        IBinaryMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option);
        IBinaryMappingBuilder WithLengthOptions(int? maxLength = null, bool? isMaxLength = null, bool? isFixedLegth = null);
        IBinaryMappingBuilder IsTimespan();
    }

    public class BinaryMappingBuilder : LengthMappingBuilder, IBinaryMappingBuilder
    {
        public BinaryMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }
        public new IBinaryMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            base.WithColumnMapping(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);
            return this;
        }

        public new IBinaryMappingBuilder WithIndex(IndexAttribute index)
        {
            base.WithIndex(index);
            return this;
        }

        public new IBinaryMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            base.HasDatabaseGeneratedOption(option);
            return this;
        }

        public new IBinaryMappingBuilder WithLengthOptions(int? maxLength = null, bool? isMaxLength = null, bool? isFixedLegth = null)
        {
            base.WithLengthOptions(maxLength, isMaxLength, isFixedLegth);
            return this;
        }

        public IBinaryMappingBuilder IsTimespan()
        {
            Property.Column.IsRowVersion = true;
            return this;
        }
    }

    public interface IDateTimeMappingBuilder : IFluentInterface
    {
        IDateTimeMappingBuilder WithColumnMapping(string columnName = null, bool? isNullable = null, string databaseType = null, int? order = null, string parameterName = null, bool? isConcurrencyToken = null);
        IDateTimeMappingBuilder WithIndex(IndexAttribute index);
        IDateTimeMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option);
        IDateTimeMappingBuilder WithPrecision(byte precision);
    }

    public class DateTimeMappingBuilder : PrimitiveMappingBuilder, IDateTimeMappingBuilder
    {
        public DateTimeMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }

        public new IDateTimeMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            base.WithColumnMapping(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);
            return this;
        }

        public new IDateTimeMappingBuilder WithIndex(IndexAttribute index)
        {
            base.WithIndex(index);
            return this;
        }

        public new IDateTimeMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            base.HasDatabaseGeneratedOption(option);
            return this;
        }

        public IDateTimeMappingBuilder WithPrecision(byte precision)
        {
            Property.Column.Precision = precision;
            return this;
        }
    }

    public interface IDecimalMappingBuilder : IFluentInterface
    {
        IDecimalMappingBuilder WithColumnMapping(string columnName = null, bool? isNullable = null, string databaseType = null, int? order = null, string parameterName = null, bool? isConcurrencyToken = null);
        IDecimalMappingBuilder WithIndex(IndexAttribute index);
        IDecimalMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option);
        IDecimalMappingBuilder WithPrecision(byte precision, byte scale);
    }

    public class DecimalMappingBuilder : PrimitiveMappingBuilder, IDecimalMappingBuilder
    {
        public DecimalMappingBuilder(PrimitivePropertyCodeModel property)
            : base(property)
        {
        }
        public new IDecimalMappingBuilder WithColumnMapping(string columnName = null,
            bool? isNullable = null,
            string databaseType = null,
            int? order = null,
            string parameterName = null,
            bool? isConcurrencyToken = null)
        {
            base.WithColumnMapping(columnName, isNullable, databaseType, order, parameterName, isConcurrencyToken);
            return this;
        }

        public new IDecimalMappingBuilder WithIndex(IndexAttribute index)
        {
            base.WithIndex(index);
            return this;
        }

        public new IDecimalMappingBuilder HasDatabaseGeneratedOption(DatabaseGeneratedOption option)
        {
            base.HasDatabaseGeneratedOption(option);
            return this;
        }

        public IDecimalMappingBuilder WithPrecision(byte precision, byte scale)
        {
            Property.Column.Precision = precision;
            Property.Column.Scale = scale;
            return this;
        }
    }
}
