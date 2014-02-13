using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Generators.Templates;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Transformations;
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
        private static readonly string DbModelBuilderParameterName = "modelBuilder";
        private static readonly string Indent = "    ";

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


        protected virtual GeneratedMappingInformation Generate(OneToOneAssociationInfo mappingInformation)
        {
            StringBuilder template = new StringBuilder();

            AssociationMemberInfo startEntity;
            AssociationMemberInfo endEntity;

            if(mappingInformation.Principal.NavigationProperty == null)
            {
                startEntity = mappingInformation.Dependent;
                endEntity = mappingInformation.Principal;
            }
            else
            {
                startEntity = mappingInformation.Principal;
                endEntity = mappingInformation.Dependent;
            }

            template.Append(DbModelBuilderParameterName).Append(".")
                .Append("Entity<")
                .Append(startEntity.ClassName)
                .Append(">()");
            PrepareNewFluentApiCall(template);

            if(endEntity.IsRequired)
            {
                template.Append("HasRequired(c => c.").Append(startEntity.NavigationProperty.Name).Append(")");
            }
            else
            {
                template.Append("HasOptional(c => c.").Append(startEntity.NavigationProperty.Name).Append(")");
            }

            PrepareNewFluentApiCall(template);

            if(startEntity.IsRequired)
            {
                if(endEntity.IsRequired)
                {
                    if(startEntity.ClassName == mappingInformation.Principal.ClassName)
                    {
                        template.Append("WithRequiredPrincipal(c => c.").Append(endEntity.NavigationProperty.Name).Append(")");
                        //WithRequiredPrincipal
                    }
                    else
                    {

                    }
                    //WithRequiredPrincipal or dependent
                }
                else
                {
                    //WithRequired
                }
            }
            else
            {
                if (endEntity.IsRequired)
                {
                    //WithOptional
                }
                else
                {
                    //WithOptionalPrincipal or dependent
                }
            }

            template.Append(";");

            return new GeneratedMappingInformation()
            {
                Value = template.ToString(),
                Type = MappingInformationType.EntityTypeConfiguration
            };
        }

        private void PrepareNewFluentApiCall(StringBuilder sb)
        {
            sb.AppendLine().Append(Indent).Append(".");
        }
    }
}
