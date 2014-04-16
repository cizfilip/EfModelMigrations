using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Model
{
    public interface IClassModelBuilder : IFluentInterface
    {
        ClassModel Class(string name, CodeModelVisibility? visibility = null, string tableName = null, string schema = null);
    }

    public sealed class ClassModelBuilder : IClassModelBuilder
    {
        public ClassModel Class(string name, CodeModelVisibility? visibility = null, string tableName = null, string schema = null)
        {
            var table = tableName != null ? new TableName(tableName, schema) : null;
            return new ClassModel(name, table, visibility);
        }
    }
}
