using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    //TODO: pri pridavani navigacni property by se melo do konstruktoru tridy pridat jeji inicializace napr:
    //public class Person
    //{
    //    public Person()
    //    {
    //        Addresses = new HashSet<Address>();
    //    }

    //    public virtual HashSet<Address> Addresses { get; set; }
    //}
    public sealed class NavigationProperty : PropertyCodeModel
    {
        public string TargetClass { get; set; }

        public bool IsCollection { get; set; }


        public NavigationProperty(string name, string targetClass, bool isCollection)
            :base(name)
        {
            this.TargetClass = targetClass;
            this.IsCollection = isCollection;
        }

        internal NavigationProperty(string targetClass, bool isCollection)
            : this(null, targetClass, isCollection)
        {
        }

        internal NavigationProperty(string targetClass)
            : this(null, targetClass, false)
        {
        }

    }
}
