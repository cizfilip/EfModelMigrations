using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping
{
    public class AddClassMapping : IAddMappingInformation
    {
        public ClassModel Model { get; private set; }
        public string[] PrimaryKeys { get; private set; }

        public AddClassMapping(ClassModel model, string[] primaryKeys)
        {
            Check.NotNull(model, "model");

            this.Model = model;
            this.PrimaryKeys = primaryKeys;
        }

        public EfFluentApiCallChain BuildEfFluentApiCallChain()
        {
            var entityCalls = new List<EfFluetApiCall>();

            if(Model.TableName != null)
            {
                var toTableCall = new EfFluetApiCall(EfFluentApiMethods.ToTable).AddParameter(new StringParameter(Model.TableName.Table));

                if (!string.IsNullOrWhiteSpace(Model.TableName.Schema))
                {
                    toTableCall.AddParameter(new StringParameter(Model.TableName.Schema));
                }

                entityCalls.Add(toTableCall);
            }
            if(PrimaryKeys != null && PrimaryKeys.Length > 0)
            {
                entityCalls.Add(new EfFluetApiCall(EfFluentApiMethods.HasKey)
                    .AddParameter(new PropertySelectorParameter(Model.Name, PrimaryKeys)));
            }

            if (entityCalls.Count > 0)
            {
                var chain = new EfFluentApiCallChain(Model.Name);
                chain.AddCalls(entityCalls);
                return chain;
            }

            return null;
        }
    }
}
