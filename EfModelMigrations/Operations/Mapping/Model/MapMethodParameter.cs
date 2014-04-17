using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class MapMethodParameter : IEfFluentApiMethodParameter
    {
        public IList<EfFluetApiCall> MapCalls { get; private set; }

        public MapMethodParameter()
        {
            MapCalls = new List<EfFluetApiCall>();
        }

        public MapMethodParameter MapKey(string[] foreignKeyColumnNames)
        {
            Check.NotNullOrEmpty(foreignKeyColumnNames, "foreignKeyColumnNames");

            MapKeyInternal(foreignKeyColumnNames, EfFluentApiMethods.MapKey);
            return this;
        }

        public MapMethodParameter MapLeftKey(string[] foreignKeyColumnNames)
        {
            Check.NotNullOrEmpty(foreignKeyColumnNames, "foreignKeyColumnNames");

            MapKeyInternal(foreignKeyColumnNames, EfFluentApiMethods.MapLeftKey);
            return this;
        }

        public MapMethodParameter MapRightKey(string[] foreignKeyColumnNames)
        {
            Check.NotNullOrEmpty(foreignKeyColumnNames, "foreignKeyColumnNames");

            MapKeyInternal(foreignKeyColumnNames, EfFluentApiMethods.MapRightKey);
            return this;
        }

        public MapMethodParameter ToTable(string tableName)
        {
            Check.NotEmpty(tableName, "tableName");

            MapCalls.Add(new EfFluetApiCall(EfFluentApiMethods.ToTable)
                .AddParameter(
                    new StringParameter(tableName)
                ));

            return this;
        }

        public MapMethodParameter HasIndexColumnAnnotation(string keyColumnName, string annotationName, IndexAttribute index)
        {
            Check.NotEmpty(keyColumnName, "keyColumnName");
            Check.NotEmpty(annotationName, "annotationName");
            Check.NotNull(index, "index");

            MapCalls.Add(new EfFluetApiCall(EfFluentApiMethods.HasColumnAnnotation)
                .AddParameter(new StringParameter(keyColumnName))
                .AddParameter(new StringParameter(annotationName))
                .AddParameter(new IndexAnnotationParameter(index)));

            return this;
        }

        private void MapKeyInternal(string[] foreignKeyColumnNames, EfFluentApiMethods methodToUse)
        {
            MapCalls.Add(new EfFluetApiCall(methodToUse)
                .AddParameters(
                    foreignKeyColumnNames.Select(k => new StringParameter(k))
                ));
        }
    }
}
