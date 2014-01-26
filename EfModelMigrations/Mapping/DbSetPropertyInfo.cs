
namespace EfModelMigrations.Mapping
{
    public class DbSetPropertyInfo : IMappingInformation
    {
        public string ClassName { get; private set; }

        public DbSetPropertyInfo(string className)
        {
            this.ClassName = className;
        }
    }
}
