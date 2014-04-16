using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations
{
    //TODO: pridat Check.<method> napric celym projektem - hlavne u public method
    //TODO: definovat metody ktere vraci nazvy DB migraci (jak pro up tak down) mozna by primo up a down metoda mohla vracet string
    public abstract class ModelMigration : IModelMigration
    {
        private List<ModelTransformation> transformations = new List<ModelTransformation>();

        public IModelMigration Model
        {
            get
            {
                return this;
            }
        }

        internal IEnumerable<ModelTransformation> Transformations
        {
            get { return transformations; }
        }

        private string id;
        public string Id
        {
            get
            {
                if (id == null)
                {
                    id = ModelMigrationsLocator.GetModelMigrationIdFromType(GetType());
                }
                return id;
            }
        }

        public string Name
        {
            get
            {
                return ModelMigrationIdGenerator.GetNameFromId(Id);
            }
        }


        public abstract void Up();
        public abstract void Down();

        public void AddTransformation(ModelTransformation transformation)
        {
            Check.NotNull(transformation, "transformation");

            transformations.Add(transformation);
        }

        internal void Reset()
        {
            transformations.Clear();
        }

    }
}
