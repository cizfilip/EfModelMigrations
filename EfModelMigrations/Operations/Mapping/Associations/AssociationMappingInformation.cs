using EfModelMigrations.Operations.Mapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO: namespace mapovacich informaci pro associace - nechat ve stejnem jako ostatni mapovaci informace nebo udelat novy??
namespace EfModelMigrations.Operations.Mapping
{
    //TODO: dodelat cascade on delete
    public abstract class AssociationMappingInformation : IMappingInformation
    {
        public string FromEntityName { get; private set; }
        public string FromNavigationProperty { get; private set; }

        public string ToEntityName { get; private set; }
        public string ToNavigationProperty { get; private set; }

        public bool WillCascadeOnDelete { get; set; }
        
        
        public AssociationMappingInformation(string fromEntityName, string fromNavigationProperty, string toEntityName, string toNavigationProperty, bool willCascadeOnDelete)
        {
            this.FromEntityName = fromEntityName;
            this.FromNavigationProperty = fromNavigationProperty;
            this.ToEntityName = toEntityName;
            this.ToNavigationProperty = toNavigationProperty;
            this.WillCascadeOnDelete = willCascadeOnDelete;
        }


        public abstract EfFluentApiCallChain BuildEfFluentApiCallChain();



        protected EfFluentApiCallChain BuildAssociationEfFluentApiCallChain(EfFluentApiMethods firstMethod, EfFluentApiMethods secondMethod)
        {
            return new EfFluentApiCallChain(FromEntityName)
                .AddMethodCall(firstMethod, CreatePropertySelectorParameter(FromEntityName, FromNavigationProperty))
                .AddMethodCall(secondMethod, CreatePropertySelectorParameter(ToEntityName, ToNavigationProperty))
                .AddMethodCall(EfFluentApiMethods.WillCascadeOnDelete, new ValueParameter(WillCascadeOnDelete));
            
        }

        protected PropertySelectorParameter CreatePropertySelectorParameter(string className, string navigationProperty)
        {
            if (string.IsNullOrEmpty(navigationProperty) || string.IsNullOrEmpty(className))
            {
                return null;
            }

            return new PropertySelectorParameter(className, navigationProperty);
        }
    }
}
