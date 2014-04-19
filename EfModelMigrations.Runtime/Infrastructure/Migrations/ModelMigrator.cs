using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Resources;
using EfModelMigrations.Transformations;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    internal class ModelMigrator : ModelMigratorBase
    {
        private HistoryTracker historyTracker;
        private ModelMigratorHelper migratorHelper;
        private Func<EfModelMetadata, IClassModelProvider> classModelProviderFactory;
        private IModelChangesExecutor modelChangesExecutor;
        private ModelMigrationsConfigurationBase configuration;
        private ModelMigrationsLocator locator;
        private VsProjectBuilder projectBuilder;
        private DbMigrationWriter dbMigrationWriter;
        private string migrationProjectPath;

        public ModelMigrator(HistoryTracker historyTracker,
            ModelMigratorHelper migratorHelper,
            Func<EfModelMetadata, IClassModelProvider> classModelProviderFactory,
            IModelChangesExecutor modelChangesExecutor,
            ModelMigrationsConfigurationBase configuration,
            VsProjectBuilder projectBuilder,
            DbMigrationWriter dbMigrationWriter,
            string migrationProjectPath)
        :base(null)
        {
            this.historyTracker = historyTracker;
            this.migratorHelper = migratorHelper;
            this.classModelProviderFactory = classModelProviderFactory;
            this.modelChangesExecutor = modelChangesExecutor;
            this.configuration = configuration;
            this.locator = new ModelMigrationsLocator(configuration);
            this.projectBuilder = projectBuilder;
            this.dbMigrationWriter = dbMigrationWriter;
            this.migrationProjectPath = migrationProjectPath;
        }


        public override void Migrate(string targetMigration, bool force)
        {
            bool isRevert;
            var migrations = locator.FindModelMigrationsToApplyOrRevert(targetMigration, out isRevert);
            base.Migrate(migrations, isRevert, force);
        }


        internal override void Migrate(IEnumerable<string> migrationIds, bool isRevert, bool force)
        {
            foreach (var migrationId in migrationIds)
            {
                base.MigrateOne(migrationId, isRevert, force);

                historyTracker.Reset();
            }
        }

        internal override void MigrateOne(string migrationId, bool isRevert, bool force)
        {
            EfModelMetadata oldEfModelMetadata;
            ScaffoldedMigration scaffoldedMigration = null;

            try
            {
                var migration = GetMigration(migrationId);
                var transformations = GetModelTransformations(migration, isRevert);

                if (transformations.Where(t => t.IsDestructiveChange).Any() && !force)
                {
                    throw new ModelMigrationsException(Strings.DataInDbMayBeLost(migration.Name));
                }

                var dbMigrationOperations = new List<MigrationOperation>();

                oldEfModelMetadata = GetEfModelMetadata();
                //proccess transformations
                foreach (var transformation in transformations)
                {
                    var modelProvider = classModelProviderFactory(oldEfModelMetadata);

                    //Verify preconditions
                    base.VerifyPreconsitions(transformation, modelProvider);

                    //apply model changes
                    base.ApplyModelChanges(transformation, modelProvider);

                    //build & get new model
                    projectBuilder.BuildModelProject();
                    var newEfModelMetadata = GetEfModelMetadata();

                    //get db migration operations & append it to list
                    var dbOperations = base.GetDbMigrationOperations(transformation, oldEfModelMetadata, newEfModelMetadata);
                    dbMigrationOperations.AddRange(dbOperations);

                    oldEfModelMetadata = newEfModelMetadata;
                }
                
                //generate db migration    
                scaffoldedMigration = base.GenerateDbMigration(dbMigrationOperations, GetDbMigrationName(migration, isRevert));
                //write migration - edmxModel is supplied because ef MigrationScaffolder see only old model before migrating
                dbMigrationWriter.Write(scaffoldedMigration, oldEfModelMetadata.Edmx);

                //build project before updating DB
                projectBuilder.BuildModelProject();

                //update DB
                base.UpdateDatabase(scaffoldedMigration.MigrationId);

                //update applied migration in configuration 
                UpdateConfiguration(migrationId, scaffoldedMigration.MigrationId, isRevert);
            }
            //TODO: Pokud dojde k vyjimce v restore na historyTrackeru tak se vyhodi jen ta a my zapomeneme duvod proc jsme vubec restorovali
            //idealni vypis:
            // 1) message vyjimky ktera zpusobila nutnost restoru!
            // 2) pokud se restore provedl ok vypsat: vsechny zmeny pred chybou byli vraceny, pokud je vyjimka i v restoru napsat ze je to uplne v haji a at si uzivatel poradi sam :)
            catch (ModelMigrationsException)
            {
                base.RollbackModelState(historyTracker, scaffoldedMigration);
                throw;
            }
            catch (Exception e)
            {
                base.RollbackModelState(historyTracker, scaffoldedMigration);
                throw new ModelMigrationsException(Strings.ApplyMigrationError, e);
            }
        }

        internal override void VerifyPreconsitions(ModelTransformation transformation, IClassModelProvider modelProvider)
        {
            var preconditions = transformation.GetPreconditions();

            foreach (var precondition in preconditions)
            {
                var result = precondition.Verify(modelProvider);
                if (!result.Success)
                {
                    string transformationName = transformation.GetType().Name.RemoveFromEnd("Transformation");
                    throw new ModelTransformationValidationException(
                        string.Concat(Strings.PreconditionFailed(transformationName), 
                        Environment.NewLine,
                        result.Message)
                    );
                }
            }
        }


        internal override void ApplyModelChanges(ModelTransformation transformation, IClassModelProvider modelProvider)
        {
            IEnumerable<IModelChangeOperation> operations = transformation.GetModelChangeOperations(modelProvider);
            modelChangesExecutor.Execute(operations);
        }

        internal override IEnumerable<MigrationOperation> GetDbMigrationOperations(ModelTransformation transformation, EfModelMetadata oldEfModelMetadata, EfModelMetadata newEfModelMetadata)
        {
            var operationBuilder = new DbMigrationOperationBuilder(new EfModel(oldEfModelMetadata), new EfModel(newEfModelMetadata));

            return transformation.GetDbMigrationOperations(operationBuilder).ToList();
        }

        internal override ScaffoldedMigration GenerateDbMigration(IEnumerable<MigrationOperation> operations, string dbMigrationName)
        {
            ((ExtendedCSharpMigrationCodeGenerator)configuration.DbMigrationsConfiguration.CodeGenerator).NewOperations = operations;

            MigrationScaffolder scaffolder = new MigrationScaffolder(configuration.DbMigrationsConfiguration);

            var scaffoldedMigration = scaffolder.Scaffold(dbMigrationName, ignoreChanges: true);

            return scaffoldedMigration;
        }

        internal override void UpdateDatabase(string dbMigrationId)
        {
            migratorHelper.UpdateDatabase(dbMigrationId);
        }

        internal override void RollbackModelState(HistoryTracker historyTracker, ScaffoldedMigration scaffoldedMigration)
        {
            historyTracker.Restore();

            //Ensure db migration is removed
            if (scaffoldedMigration != null)
            {
                dbMigrationWriter.RemoveMigration(scaffoldedMigration);
            }
        }

        private IEnumerable<ModelTransformation> GetModelTransformations(ModelMigration migration, bool isRevert)
        {
            migration.Reset();
            if (isRevert)
            {
                migration.Down();
            }
            else
            {
                migration.Up();
            }

            return migration.Transformations;
        }

        private ModelMigration GetMigration(string migrationId)
        {
            var modelMigrationType = locator.FindModelMigration(migrationId);
            return modelMigrationType.CreateInstance<ModelMigration>();
        }

        private string GetDbMigrationName(ModelMigration migration, bool isRevert)
        {
            string dbMigrationName = migration.Name;
            if (isRevert)
            {
                dbMigrationName = "Revert" + dbMigrationName;
            }
            return dbMigrationName;
        }

        private void UpdateConfiguration(string appliedModelMigrationId, string appliedDbMigrationId, bool isRevert)
        {
            //TODO: updatovat na projekt kde jsou migrace ne modelProject - az budu podporovat vicero projektu
            string configurationResourcePath = Path.Combine(migrationProjectPath, configuration.ModelMigrationsDirectory, configuration.GetType().Name + ".resx");
            var updater = new ModelMigrationsConfigurationUpdater(configurationResourcePath);

            if (!isRevert)
            {
                updater.AddAppliedMigration(appliedModelMigrationId, appliedDbMigrationId);
            }
            else
            {
                updater.RemoveLastAppliedMigration(appliedDbMigrationId);
            }
        }

        private EfModelMetadata GetEfModelMetadata()
        {
            string edmx = migratorHelper.GetEdmxModel();
            return EfModelMetadata.Load(edmx);
        }
    }
}
