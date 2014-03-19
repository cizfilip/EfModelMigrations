using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    internal class ParametersParser
    {
        //TODO: Dat stringy vyjimek do resourcu
        public static IEnumerable<ScalarProperty> ParseProperties(IEnumerable<string> parameters)
        {
            foreach (var param in parameters)
            {
                yield return ParsePropertyModel(param);
            }
        }

        //TODO: Dat stringy vyjimek do resourcu
        private static ScalarProperty ParsePropertyModel(string param)
        {
            var splitted = param.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (splitted.Length != 2)
            {
                throw new ModelMigrationsException("Wrong property format, use [PropertyName]:[PropertyType], example: Name:string ");
            }

            ScalarProperty property;
            if (!ScalarProperty.TryParse(splitted[1], out property))
            {
                throw new ModelMigrationsException(string.Format("Unknown scalar property type {0}", splitted[1]));
            }

            property.Name = splitted[0];

            return property;
        }


    }
}
