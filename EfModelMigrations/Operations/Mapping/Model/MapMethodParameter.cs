using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            MapKeyInternal(foreignKeyColumnNames, EfFluentApiMethods.MapKey);
            return this;
        }

        public MapMethodParameter MapLeftKey(string[] foreignKeyColumnNames)
        {
            MapKeyInternal(foreignKeyColumnNames, EfFluentApiMethods.MapLeftKey);
            return this;
        }

        public MapMethodParameter MapRightKey(string[] foreignKeyColumnNames)
        {
            MapKeyInternal(foreignKeyColumnNames, EfFluentApiMethods.MapRightKey);
            return this;
        }

        public MapMethodParameter ToTable(string tableName)
        {
            MapCalls.Add(new EfFluetApiCall(EfFluentApiMethods.ToTable)
                .AddParameter(
                    new StringParameter(tableName)
                ));

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
