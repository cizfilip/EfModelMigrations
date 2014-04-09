using EfModelMigrations.Infrastructure.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Transformations.Model
{
    public sealed class ClassModel
    {
        //TODO: baseType a implementedinterfaces - ano nebo ne??

        public string Name { get; private set; }
        public CodeModelVisibility? Visibility { get; private set; }
        public TableName TableName { get; private set; }       

        public ClassModel(string name, 
            TableName tableName = null,
            CodeModelVisibility? visibility = null)
        {
            Check.NotEmpty(name, "name");

            this.Name = name;
            this.TableName = tableName;
            this.Visibility = visibility;
        }
    }

    public sealed class ClassModelBuilder
    {
        public ClassModel Class(string name, CodeModelVisibility? visibility = null, string tableName = null, string schema = null)
        {
            return new ClassModel(name, new TableName(tableName, schema), visibility);
        }
    }
}
