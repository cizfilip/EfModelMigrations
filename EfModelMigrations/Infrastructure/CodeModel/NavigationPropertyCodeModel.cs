using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public class NavigationPropertyCodeModel : ClassMemberCodeModel
    {
        public NavigationPropertyCodeModel() : base()
        {
            this.IsCollection = false;

        }

        public string TargetClass { get; set; }

        public bool IsCollection { get; set; }



        public PropertyCodeModel ToPropertyCodeModel()
        {
            return new PropertyCodeModel()
            {
                IsSetterPrivate = this.IsSetterPrivate,
                Name = this.Name,
                Visibility = this.Visibility,
                Type = GetPropertyType()
            };
        }

        private string GetPropertyType()
        {
            if(IsCollection)
            {
                //TODO: zrefaktorovat nekam jinam jelikoz je to C# specifik generovani. a melo by byt nastavitelne zda budou nav properties ICollection, IList, List atd...
                return string.Format("ICollection<{0}>", TargetClass);
            }
            else
            {
                return TargetClass;
            }
        }



        public static NavigationPropertyCodeModel Default(string targetClass)
        {
            return new NavigationPropertyCodeModel()
            {
                Name = targetClass,
                TargetClass = targetClass
            };
        }
    }
}
