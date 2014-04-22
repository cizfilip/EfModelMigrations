using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Utilities;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Infrastructure
{
    internal sealed class ModelMigrationsLocator
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
                throw new ModelMigrationsException(Strings.CannotFindMigration(modelMigrationId));
            }
        }

        public string UniquifyMigrationName(string migrationName)
        {
            return modelMigrations.Select(m => m.Value.GetType().Name).Uniquify(migrationName);
        }

        public IEnumerable<string> FindModelMigrationsToApplyOrRevert(string targetMigration, out bool isRevert)
        {
            //if target not specified return all pending migrations
            if (string.IsNullOrEmpty(targetMigration))
            {
                isRevert = false;
                return GetPendingMigrationsIds().ToList();
            }

            string targetMigrationId = GetMigrationId(targetMigration);

            //if target is InitialModel
            if (targetMigrationId == ModelMigrationIdGenerator.InitialModel)
            {
                isRevert = true;
                return GetAppliedMigrationsIds().Reverse().ToList();
            }

            var appliedModelMigrations = GetAppliedMigrationsIds();
            //check if fromMigration is same as last appliedMigrations
            if (targetMigrationId.EqualsOrdinal(appliedModelMigrations.LastOrDefault()))
            {
                isRevert = false;
                return Enumerable.Empty<string>();
            }

            //see if fromMigration is in applied migrations
            var migrationsToUnapply = appliedModelMigrations.SkipWhile(m => !m.EqualsOrdinal(targetMigrationId));
            if (migrationsToUnapply.Any())
            {
                isRevert = true;
                //Skip target migration
                return migrationsToUnapply.Skip(1).Reverse().ToList();
            }

            //see if fromMigration is in pending migrations
            var pendingMigrations = GetPendingMigrationsIds();
            if (pendingMigrations.Contains(targetMigrationId, StringComparer.Ordinal))
            {
                //return all pending migration including from migration
                isRevert = false;
                return pendingMigrations.TakeWhile(m => !m.EqualsOrdinal(targetMigrationId))
                        .Concat(new[] { targetMigrationId })
                        .ToList();
            }

            //if we reach here there is nothing to apply or revert
            isRevert = false;
            return Enumerable.Empty<string>();
        }

        private string GetMigrationId(string migrationName)
        {

            if (ModelMigrationIdGenerator.IsValidId(migrationName))
            {
                return migrationName;
            }

            var possibleMigrationIds = GetModelMigrationsIds().Where(m => ModelMigrationIdGenerator.GetNameFromId(m).EqualsOrdinalIgnoreCase(migrationName)).ToArray();

            if(possibleMigrationIds.Length != 1)
            {
                throw new ModelMigrationsException(Strings.CannotFindMigration(migrationName));
            }
            if (possibleMigrationIds.Length > 1)
            {
                throw new ModelMigrationsException(Strings.MultipleMigrationsFound(migrationName));
            }

            return possibleMigrationIds.Single();
        }

        public IEnumerable<string> GetPendingMigrationsIds()
        {
            return GetModelMigrationsIds().Except(appliedModelMigrations, StringComparer.Ordinal);
        }

        private IEnumerable<string> GetAppliedMigrationsIds()
        {
            return appliedModelMigrations;
        }


        private IEnumerable<string> GetModelMigrationsIds()
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
                throw new ModelMigrationsException(Strings.CannotFindMigrationId(migrationType.Name), e);
            }

            if (string.IsNullOrEmpty(id) || !ModelMigrationIdGenerator.IsValidId(id))
            {
                throw new ModelMigrationsException(Strings.InvalidModelMigrationId);
            }
            return id;
        }

        #endregion
    }
}
