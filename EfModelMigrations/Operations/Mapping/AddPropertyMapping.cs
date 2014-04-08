using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Operations.Mapping.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class AddPropertyMapping : IAddMappingInformation
    {
        public string ClassName { get; private set; }
        public PrimitivePropertyCodeModel Property { get; private set; }

        public AddPropertyMapping(string className, PrimitivePropertyCodeModel property)
        {
            Check.NotEmpty(className, "className");
            Check.NotNull(property, "property");

            this.ClassName = className;
            this.Property = property;
        }

        public EfFluentApiCallChain BuildEfFluentApiCallChain()
        {
            var propertyCalls = new List<EfFluetApiCall>();

            if(Property is EnumPropertyCodeModel)
            {
                BuildPrimitiveCalls(propertyCalls);
            }
            else if(Property is ScalarPropertyCodeModel)
            {
                var propertyType = (Property as ScalarPropertyCodeModel).Type;

                if(propertyType == PrimitiveTypeKind.Binary)
                {
                    BuildBinaryCalls(propertyCalls);
                } 
                else if (propertyType == PrimitiveTypeKind.String)
                {
                    BuildStringCalls(propertyCalls);
                }
                else if (propertyType == PrimitiveTypeKind.Decimal)
                {
                    BuildDecimalCalls(propertyCalls);
                }
                else if (propertyType == PrimitiveTypeKind.DateTime || 
                    propertyType == PrimitiveTypeKind.DateTimeOffset || 
                    propertyType == PrimitiveTypeKind.Time)
                {
                    BuildDateTimeCalls(propertyCalls);
                }
                else
                {
                    BuildPrimitiveCalls(propertyCalls);
                }
            }

            if (propertyCalls.Count > 0)
            {
                var chain = new EfFluentApiCallChain(ClassName);
                chain.AddMethodCall(EfFluentApiMethods.Property, new PropertySelectorParameter(ClassName, Property.Name));
                chain.AddCalls(propertyCalls);
                return chain;
            }

            return null;
        }

        private void BuildPrimitiveCalls(IList<EfFluetApiCall> calls)
        {
            var column = Property.Column;

            if(column.IsNullable.HasValue) //required / optional
            {
                if(column.IsNullable.Value)
                {
                    calls.Add(new EfFluetApiCall(EfFluentApiMethods.IsOptional));
                }
                else
                {
                    calls.Add(new EfFluetApiCall(EfFluentApiMethods.IsRequired));
                }
            }
            if(!string.IsNullOrWhiteSpace(column.ColumnName)) // column name
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasColumnName).AddParameter(new StringParameter(column.ColumnName)));
            }
            if (!string.IsNullOrWhiteSpace(column.ColumnType)) // column type
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasColumnType).AddParameter(new StringParameter(column.ColumnType)));
            }
            if (column.ColumnOrder.HasValue) // column order
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasColumnOrder).AddParameter(new ValueParameter(column.ColumnOrder.Value)));
            }
            if (column.ColumnAnnotations != null && column.ColumnAnnotations.Count > 0) // column annotations
            {
                foreach (var annotation in column.ColumnAnnotations)
                {
                    //TODO: umim mapovat jen index annotaci - coz mi staci a asi ani neni mozne mapovat obecne jakkoukoliv...
                    if(annotation.Value is IndexAnnotation)
                    {
                        var indexAnnotation = annotation.Value as IndexAnnotation;
                        calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasColumnAnnotation)
                            .AddParameter(new StringParameter(annotation.Key))
                            .AddParameter(new IndexAnnotationParameter(indexAnnotation.Indexes.ToArray())));
                    }
                }
            }
            if (column.DatabaseGeneratedOption.HasValue) //DatabaseGeneratedOption
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasDatabaseGeneratedOption)
                    .AddParameter(new EnumParameter(typeof(DatabaseGeneratedOption), (int)column.DatabaseGeneratedOption.Value)));
            }
            if (!string.IsNullOrWhiteSpace(column.ParameterName)) // parameter name
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasParameterName).AddParameter(new StringParameter(column.ParameterName)));
            }
            if (column.IsConcurrencyToken.HasValue) // concurency token
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.IsConcurrencyToken).AddParameter(new ValueParameter(column.IsConcurrencyToken.Value)));
            }
        }

        private void BuildLengthCalls(IList<EfFluetApiCall> calls)
        {
            var column = Property.Column;

            if (column.IsMaxLength.HasValue) //is max length
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.IsMaxLength));
            }
            if (column.MaxLength.HasValue) //max length
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasMaxLength).AddParameter(new ValueParameter(column.MaxLength.Value)));
            }
            if(column.IsFixedLength.HasValue) //fixed or variable length
            {
                if(column.IsFixedLength.Value)
                {
                    calls.Add(new EfFluetApiCall(EfFluentApiMethods.IsFixedLength));
                }
                else
                {
                    calls.Add(new EfFluetApiCall(EfFluentApiMethods.IsVariableLength));
                }
            }

            BuildPrimitiveCalls(calls);
        }

        private void BuildStringCalls(IList<EfFluetApiCall> calls)
        {
            var column = Property.Column;

            if (column.IsUnicode.HasValue) //is unicode
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.IsUnicode).AddParameter(new ValueParameter(column.IsUnicode.Value)));
            }

            BuildLengthCalls(calls);
        }
        private void BuildBinaryCalls(IList<EfFluetApiCall> calls)
        {
            var column = Property.Column;

            if (column.IsRowVersion.HasValue && column.IsRowVersion.Value) //is row version
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.IsRowVersion));
            }

            BuildLengthCalls(calls);
        }

        private void BuildDateTimeCalls(IList<EfFluetApiCall> calls)
        {
            var column = Property.Column;

            if (column.Precision.HasValue) //precision
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasPrecision).AddParameter(new ValueParameter(column.Precision.Value)));
            }
            
            BuildPrimitiveCalls(calls);
        }
        private void BuildDecimalCalls(IList<EfFluetApiCall> calls)
        {
            var column = Property.Column;

            if (column.Precision.HasValue && column.Scale.HasValue) //precision and scale
            {
                calls.Add(new EfFluetApiCall(EfFluentApiMethods.HasPrecision)
                    .AddParameter(new ValueParameter(column.Precision.Value))
                    .AddParameter(new ValueParameter(column.Scale.Value)));
            }

            BuildPrimitiveCalls(calls);
        }
    }
}
