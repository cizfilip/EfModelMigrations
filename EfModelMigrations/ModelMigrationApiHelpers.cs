using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.CodeModel.Builders;
using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations
{
    public static class ModelMigrationApiHelpers
    {
        public static IEnumerable<PrimitivePropertyCodeModel> ConvertObjectToPrimitivePropertyModel<TProps>(TProps properties)
        {
            var propertiesOnObject = properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(
                p => !p.GetIndexParameters().Any());

            foreach (var property in propertiesOnObject)
            {
                var mappingBuilder = property.GetValue(properties) as PrimitiveMappingBuilder;

                var primitiveProperty = mappingBuilder != null ? mappingBuilder.Property : null;

                if (primitiveProperty == null)
                    throw new ModelMigrationsException(Strings.PropertyDefinitionExtractionFailed);

                if (string.IsNullOrWhiteSpace(primitiveProperty.Name))
                {
                    primitiveProperty.Name = property.Name;
                }

                yield return primitiveProperty;
            }
        }

        public static IEnumerable<ForeignKeyPropertyCodeModel> ConvertObjectToForeignKeyPropertyModel<TProps>(TProps properties)
        {
            var propertiesOnObject = properties.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(
                p => !p.GetIndexParameters().Any());

            foreach (var property in propertiesOnObject)
            {
                var fkProperty = property.GetValue(properties) as ForeignKeyPropertyCodeModel;

                if (fkProperty == null)
                    throw new ModelMigrationsException(Strings.ForeignKeyDefinitionExtractionFailed);

                if (string.IsNullOrWhiteSpace(fkProperty.Name))
                {
                    fkProperty.Name = property.Name;
                }

                yield return fkProperty;
            }
        }

    }
}
