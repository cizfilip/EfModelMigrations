using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public static class AssociationCodeModelExtensions
    {
        public static bool? GetWillCascadeOnDelete(this AssociationCodeModel model)
        {
            var info = model.GetInformation<bool>(AssociationInfo.WillCascadeOnDelete);
            return info == null ? (bool?)null : info.Value;
        }

        public static ManyToManyJoinTable GetJoinTable(this AssociationCodeModel model)
        {
            var info = model.GetInformation<ManyToManyJoinTable>(AssociationInfo.JoinTable);
            return info == null ? null : info.Value;
        }

        public static string[] GetForeignKeyColumnNames(this AssociationCodeModel model)
        {
            var info = model.GetInformation<string[]>(AssociationInfo.ForeignKeyColumnNames);
            return info == null ? null : info.Value;
        }

        public static ForeignKeyPropertyCodeModel[] GetForeignKeyProperties(this AssociationCodeModel model)
        {
            var info = model.GetInformation<ForeignKeyPropertyCodeModel[]>(AssociationInfo.ForeignKeyProperties);
            return info == null ? null : info.Value;
        }

        public static IndexAttribute GetForeignKeyIndex(this AssociationCodeModel model)
        {
            var info = model.GetInformation<IndexAttribute>(AssociationInfo.ForeignKeyIndex);
            return info == null ? null : info.Value;
        }
    }
}
