using EfModelMigrations.Operations.Mapping.Model;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO: namespace mapovacich informaci pro associace - nechat ve stejnem jako ostatni mapovaci informace nebo udelat novy??
namespace EfModelMigrations.Operations.Mapping
{
    public abstract class AssociationInfo : IMappingInformation
    {
        public AssociationMemberInfo Principal { get; private set; }

        public AssociationMemberInfo Dependent { get; private set; }

        

        public AssociationInfo(AssociationMemberInfo principal, AssociationMemberInfo dependent)
        {
            this.Principal = principal;
            this.Dependent = dependent;
        }


        public EfFluentApiCallChain BuildEfFluentApiCallChain()
        {
            EfFluentApiCallChain callChain;

            if (Principal.NavigationProperty != null) //Navigation property on Principal
            {
                var methods = GetFluentApiMethodsStartingFromPrincipal();
                callChain = new EfFluentApiCallChain(Principal.ClassName)
                    .AddMethodCall(methods.Item1, CreatePropertySelectorParameter(Principal.ClassName, Principal.NavigationPropertyName))
                    .AddMethodCall(methods.Item2, CreatePropertySelectorParameter(Dependent.ClassName, Dependent.NavigationPropertyName));
            }
            else //Navigation property on Dependent
            {
                var methods = GetFluentApiMethodsStartingFromDependent();
                callChain = new EfFluentApiCallChain(Dependent.ClassName)
                    .AddMethodCall(methods.Item1, CreatePropertySelectorParameter(Dependent.ClassName, Dependent.NavigationPropertyName))
                    .AddMethodCall(methods.Item2, CreatePropertySelectorParameter(Principal.ClassName, Principal.NavigationPropertyName));
            }

            AddAdditionalMethodCalls(callChain);            

            return callChain;
        }


        protected abstract Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromPrincipal();

        protected abstract Tuple<EfFluentApiMethods, EfFluentApiMethods> GetFluentApiMethodsStartingFromDependent();
        
        protected abstract void AddAdditionalMethodCalls(EfFluentApiCallChain callChain);
        
        protected virtual PropertySelectorParameter CreatePropertySelectorParameter(string className, string navigationProperty)
        {
            if (string.IsNullOrEmpty(navigationProperty) || string.IsNullOrEmpty(className))
            {
                return null;
            }

            return new PropertySelectorParameter(className, navigationProperty);
        }
    }
}
