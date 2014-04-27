using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.CodeModel
{
    public abstract class AssociationInfo
    {
        public static readonly string WillCascadeOnDelete = "WillCascadeOnDelete";
        public static readonly string JoinTable = "JoinTable";
        public static readonly string ForeignKeyColumnNames = "ForeignKeyColumnNames";
        public static readonly string ForeignKeyProperties = "ForeignKeyProperties";
        public static readonly string ForeignKeyIndex = "ForeignKeyIndex";

        public string Name { get; private set; }

        internal AssociationInfo(string name)
        {
            this.Name = name;
        }

        public abstract object GetValue();


        public static AssociationInfo<bool> CreateWillCascadeOnDelete(bool value)
        {
            return new AssociationInfo<bool>(WillCascadeOnDelete, value);
        }

        public static AssociationInfo<ManyToManyJoinTable> CreateJoinTable(ManyToManyJoinTable value)
        {
            Check.NotNull(value, "value");

            return new AssociationInfo<ManyToManyJoinTable>(JoinTable, value);
        }

        public static AssociationInfo<string[]> CreateForeignKeyColumnNames(string[] value)
        {
            Check.NotNullOrEmpty(value, "value");

            return new AssociationInfo<string[]>(ForeignKeyColumnNames, value);
        }

        public static AssociationInfo<ForeignKeyPropertyCodeModel[]> CreateForeignKeyProperties(ForeignKeyPropertyCodeModel[] value)
        {
            Check.NotNullOrEmpty(value, "value");

            return new AssociationInfo<ForeignKeyPropertyCodeModel[]>(ForeignKeyProperties, value);
        }

        public static AssociationInfo<IndexAttribute> CreateForeignKeyIndex(IndexAttribute value)
        {
            Check.NotNull(value, "value");

            return new AssociationInfo<IndexAttribute>(ForeignKeyIndex, value);
        }
    }

    public class AssociationInfo<T> : AssociationInfo
    {
        public T Value { get; private set; }

        public AssociationInfo(string name, T value)
            : base(name)
        {
            this.Value = value;
        }

        public override object GetValue()
        {
            return Value;
        }
    }
}
