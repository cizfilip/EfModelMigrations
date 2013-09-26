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
        private readonly List<ModelTransformation> transformations = new List<ModelTransformation>();

        internal IEnumerable<ModelTransformation> Transformations
        {
            get { return transformations; }
        }

        public string Id { get; protected set; }




        public abstract void Up();
        public abstract void Down();

        internal void AddTransformation(ModelTransformation transformation)
        {
            transformations.Add(transformation);
        }

        
    }
}
