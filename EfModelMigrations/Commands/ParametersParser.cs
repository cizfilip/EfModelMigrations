using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Commands
{
    public class ParametersParser
    {
        private static readonly Dictionary<string, CodeModelVisibility> visibilityParser = new Dictionary<string, CodeModelVisibility>()
            {
                {"private", CodeModelVisibility.Private},
                {"public", CodeModelVisibility.Public},
                {"protected", CodeModelVisibility.Protected},
                {"internal", CodeModelVisibility.Internal},
                {"protectedinternal", CodeModelVisibility.ProtectedInternal}
            };

        private IClassModelProvider modelProvider;

        public ParametersParser(IClassModelProvider modelProvider)
        {
            this.modelProvider = modelProvider;
        }

        public CodeModelVisibility ParseVisibility(string visibility)
        {
            Check.NotEmpty(visibility, "visibility");

            if(visibilityParser.ContainsKey(visibility.ToLowerInvariant()))
            {
                return visibilityParser[visibility];
            }

            throw new ModelMigrationsException(Strings.ParameterParser_InvalidVisibility(visibility, string.Join(", ", visibilityParser.Keys))); 
        }

        public IEnumerable<PrimitivePropertyCodeModel> ParseProperties(IEnumerable<string> parameters)
        {
            Check.NotNullOrEmpty(parameters, "parameters");

            foreach (var param in parameters)
            {
                yield return ParseProperty(param);
            }
        }

        public PrimitivePropertyCodeModel ParseProperty(string parameter)
        {
            Check.NotEmpty(parameter, "parameter");

            var splitted = parameter.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (splitted.Length != 2 || string.IsNullOrWhiteSpace(splitted[0]) || string.IsNullOrWhiteSpace(splitted[1]))
            {
                throw new ModelMigrationsException(Strings.ParameterParser_WrongPropertyFormat);
            }

            string name = splitted[0];
            string type = splitted[1];

            ScalarPropertyCodeModel property;
            if (ScalarPropertyCodeModel.TryParse(name, type, out property))
            {
                return property;
            }

            string enumType;
            var isNullable = PrimitivePropertyCodeModel.TryUnwrapNullability(type, out enumType);
            //Try find if its enum
            if (modelProvider.IsEnumInModel(enumType))
            {
                return new EnumPropertyCodeModel(enumType, isNullable);
            }

            throw new ModelMigrationsException(Strings.ParameterParser_UnknownPropertyType(splitted[1])); 
        }

       
    }
}
