using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Properties;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations
{
    public abstract class ModelMigration
    {
        private List<ModelTransformation> transformations = new List<ModelTransformation>();

        //TODO: udelat internal az bude kod z runneru ve vlastnich tridach v hlavnich projektu
        public IEnumerable<ModelTransformation> Transformations
        {
            get { return transformations; }
        }

        private string id;
        public string Id
        {
            get
            {
                if( id == null)
                {
                    id = ModelMigrationsLocator.GetModelMigrationIdFromType(GetType());
                }
                return id;
            }
        }
        

        public abstract void Up();
        public abstract void Down();

        //TODO: internal or public??
        internal void AddTransformation(ModelTransformation transformation)
        {
            transformations.Add(transformation);
        }

        //TODO: internal or public??
        public void Reset()
        {
            transformations.Clear();
        }

        //TODO: must have private or internal setter
        public IClassModelProvider ClassModelProvider { get; set; }
        


    }
}
