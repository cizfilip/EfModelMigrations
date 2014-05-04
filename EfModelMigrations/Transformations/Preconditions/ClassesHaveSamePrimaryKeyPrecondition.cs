using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Preconditions
{
    public class ClassesHaveSamePrimaryKeyPrecondition : ModelTransformationPrecondition
    {
        private string[] classes;

        public ClassesHaveSamePrimaryKeyPrecondition(string[] classes)
        {
            Check.NotNullOrEmpty(classes, "classes");

            this.classes = classes;
        }

        public override VerificationResult Verify(IClassModelProvider modelProvider)
        {
            try
            {
                var primaryKeys = new PrimitivePropertyCodeModel[classes.Length][];
                
                for (int i = 0; i < classes.Length; i++)
                {
                    var classModel = modelProvider.GetClassCodeModel(@classes[i]);
                    primaryKeys[i] = classModel.PrimaryKeys.ToArray();
                }
               
                if (primaryKeys.Select(k => k.Length).Distinct().Count() != 1) // classes have same number of primary keys
                {
                    return GetErrorResult();
                }

                //deep check
                for (int i = 0; i < primaryKeys.Length; i++)
                {
                    for (int j = i + 1; j < primaryKeys.Length; j++)
                    {
                        if (!IsSameProperties(primaryKeys[i], primaryKeys[j]))
                        {
                            return GetErrorResult();
                        }
                    }
                }
            }
            catch (Exception) { }

            return VerificationResult.Successful();
        }

        //TODO: compare properties not only their names...
        private bool IsSameProperties(PrimitivePropertyCodeModel[] properties, PrimitivePropertyCodeModel[] otherProperties)
        {
            if (properties.Length != otherProperties.Length)
            {
                return false;
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];
                var otherProp = otherProperties[i];

                if (!string.Equals(prop.Name, otherProp.Name, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private VerificationResult GetErrorResult()
        {
            return VerificationResult.Error(Strings.Precondition_ClassesNotHaveSamePK(string.Join(", ", classes)));
        }
    }
}
