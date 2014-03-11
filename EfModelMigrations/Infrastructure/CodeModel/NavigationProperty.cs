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
    public class NavigationProperty : PropertyCodeModel<NavigationType>
    {
        public NavigationProperty(NavigationType type) 
            : base(type)
        {
        }

        public NavigationProperty(string name, NavigationType type)
            : base(name, type)
        {
        }

        



        //public ScalarProperty ToPropertyCodeModel()
        //{
        //    return new ScalarProperty()
        //    {
        //        IsSetterPrivate = this.IsSetterPrivate,
        //        Name = this.Name,
        //        Visibility = this.Visibility,
        //        Type = GetPropertyType(),
        //        IsVirtual = this.IsVirtual
        //    };
        //}

        //private string GetPropertyType()
        //{
        //    if(IsCollection)
        //    {
        //        //TODO: zrefaktorovat nekam jinam jelikoz je to C# specifik generovani. a melo by byt nastavitelne zda budou nav properties ICollection, IList, List atd...
        //        return string.Format("ICollection<{0}>", TargetClass);
        //    }
        //    else
        //    {
        //        return TargetClass;
        //    }
        //}



        public static NavigationProperty Default(string targetClass)
        {
            return new NavigationProperty(targetClass, new NavigationType(targetClass, false));
        }

        public static NavigationProperty DefaultCollection(string targetClass)
        {
            return new NavigationProperty(targetClass, new NavigationType(targetClass, true));
        }
    }
}
