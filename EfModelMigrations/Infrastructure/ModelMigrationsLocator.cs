using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Properties;
using EfModelMigrations.Utilities;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EfModelMigrations.Infrastructure
{
    //TODO: Musime mit metodu co zajisti unikatnost jmen migraci
    public class ModelMigrationsLocator
    {
        /// <summary>
        /// Holds this keyvalue pairs: (modelMigrationId -> Type represent modelmigration)
        /// </summary>
        private Dictionary<string, Type> modelMigrations;
        private List<string> appliedModelMigrations;
        
        public ModelMigrationsLocator(ModelMigrationsConfigurationBase configuration)
        {
            this.appliedModelMigrations = configuration.GetAppliedMigrations().ToList();

            var typeFinder = new TypeFinder();
            var migrationsNamespace = configuration.ModelMigrationsNamespace;
            var migrationsAssembly = configuration.ModelMigrationsAssembly;

            modelMigrations = typeFinder.FindTypes(migrationsAssembly, typeof(ModelMigration))
                .Where(t => t.Namespace.EqualsOrdinal(migrationsNamespace))
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .Where(t => !t.IsAbstract && !t.IsGenericType)
                .Select(t => new
                {
                    ModelMigrationId = ModelMigrationsLocator.GetModelMigrationIdFromType(t),
                    ModelMigrationType = t
                })
                .OrderBy(t => t.ModelMigrationId)
                .ToDictionary(t => t.ModelMigrationId, t => t.ModelMigrationType);
        }

        public Type FindModelMigration(string modelMigrationId)
        {
            Type modelMigrationType;
            if (modelMigrations.TryGetValue(modelMigrationId, out modelMigrationType))
            {
                return modelMigrationType;
            }
            else
            {
                throw new ModelMigrationsException(string.Format(Resources.CannotFindMigration, modelMigrationId));
            }
        }

        public IEnumerable<string> GetPendingMigrationsIds()
        {
            return GetModelMigrationsIds().Except(appliedModelMigrations, StringComparer.Ordinal);
        }

        public IEnumerable<string> GetAppliedMigrationsIds()
        {
            return appliedModelMigrations;
        }
        

        public IEnumerable<string> GetModelMigrationsIds()
        {
            return modelMigrations.Keys;
        }


        #region Static methods
        
        public static string GetModelMigrationIdFromType(Type migrationType)
        {
            string id;
            try
            {
                id = migrationType.GetCustomAttributes(typeof(ModelMigrationIdAttribute), inherit: false).Cast<ModelMigrationIdAttribute>().Single().Id;
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(Resources.CannotFindMigrationId, e);
            }

            if (string.IsNullOrEmpty(id) || !ModelMigrationIdGenerator.IsValidId(id))
            {
                throw new ModelMigrationsException(Resources.InvalidModelMigrationId);
            }
            return id;
        }

        #endregion
    }
}
