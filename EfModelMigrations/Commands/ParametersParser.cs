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
        public static IEnumerable<PropertyCodeModel> ParseProperties(IEnumerable<string> parameters)
        {
            foreach (var param in parameters)
            {
                yield return ParsePropertyModel(param);
            }
        }

        //TODO: Dat stringy vyjimek do resourcu
        private static PropertyCodeModel ParsePropertyModel(string param)
        {
            var splitted = param.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (splitted.Length != 2)
            {
                throw new ModelMigrationsException("Wrong property format, use [PropertyName]:[PropertyType], example: Name:string ");
            }

            return new PropertyCodeModel()
            {
                Name = splitted[0],
                Type = splitted[1]
            };

        }
    }
}
