using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Generators.Templates;
using EfModelMigrations.Operations.Mapping;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.Generators
{
    public class CSharpMappingInformationsGenerator : IMappingInformationsGenerator
    {
        public virtual GeneratedMappingInformation Generate(IMappingInformation mappingInformation)
        {
            dynamic mappingInfo = mappingInformation;

            try
            {
                var result = Generate(mappingInfo);
                return result;
            }
            catch (RuntimeBinderException e)
            {
                //TODO: string do resourcu
                throw new ModelMigrationsException(string.Format("Cannot generate code for mapping information {0}. Generator implementation is missing.", mappingInfo.GetType().Name), e);
            }
        }


        protected virtual GeneratedMappingInformation Generate(DbSetPropertyInfo mappingInformation)
        {
            //TODO: Takhle jde delat dbset property bez public setteru
            //public DbSet<Blog> Blogs
            //{
            //    get { return Set<Blog>(); }
            //}

            //TODO: Pluralizovat jmeno pomoci EF
            //private IPluralizationService _pluralizationService
            //= DbConfiguration.DependencyResolver.GetService<IPluralizationService>();

            //TODO: do DbSetPropertyInfo pridat property i pro nazev generované dbset property - defaultně do ní 
            //v transformaci create class dávat pluralizované jméno třídy

            string generatedValue = new DbSetPropertyTemplate()
            {
                GenericType = mappingInformation.ClassName,
                Name = mappingInformation.ClassName + "Set" //TODO: asi spis pluralizovat jmeno
            }.TransformText();

            return new GeneratedMappingInformation()
            {
                Type = MappingInformationType.DbContextProperty,
                Value = generatedValue
            };
        }
    }
}
