using EfModelMigrations.Infrastructure;
using EfModelMigrations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Runners.TypeFinders
{
    [Serializable]
    internal class FindMigrationsToApplyOrRevert : BaseRunner
    {
        public string TargetMigration { get; set; }

        public override void Run()
        {
            var locator = new ModelMigrationsLocator(Configuration);

            //if target not specified return all pending migrations
            if (string.IsNullOrEmpty(TargetMigration))
            {
                Return(new MigrationsToApplyOrRevertResult(locator.GetPendingMigrationsIds().ToList(), isRevert: false));
                return;
            }

            string targetMigrationId = locator.GetMigrationId(TargetMigration);

            //if target is InitialModel
            if(targetMigrationId == ModelMigrationIdGenerator.InitialModel)
            {
                Return(new MigrationsToApplyOrRevertResult(locator.GetAppliedMigrationsIds().Reverse().ToList(), isRevert: true));
                return;
            }

            var appliedModelMigrations = locator.GetAppliedMigrationsIds();
            //check if fromMigration is same as last appliedMigrations
            if (targetMigrationId.EqualsOrdinal(appliedModelMigrations.LastOrDefault()))
            {
                Return(MigrationsToApplyOrRevertResult.Empty);
                return;
            }

            //see if fromMigration is in applied migrations
            var migrationsToUnapply = appliedModelMigrations.SkipWhile(m => !m.EqualsOrdinal(targetMigrationId));
            if (migrationsToUnapply.Any())
            {
                Return(new MigrationsToApplyOrRevertResult(migrationsToUnapply.Reverse().ToList(), isRevert: true));
                return;
            }

            //see if fromMigration is in pending migrations
            var pendingMigrations = locator.GetPendingMigrationsIds();
            if (pendingMigrations.Contains(targetMigrationId, StringComparer.Ordinal))
            {
                //return all pending migration including from migration
                Return(new MigrationsToApplyOrRevertResult(
                    pendingMigrations.TakeWhile(m => !m.EqualsOrdinal(targetMigrationId))
                        .Concat(new[] { targetMigrationId })
                        .ToList(),
                    isRevert: true));
                return;
            }

            //if we reach here there is nothing to apply or revert
            Return(MigrationsToApplyOrRevertResult.Empty);
        }
    }

    [Serializable]
    internal class MigrationsToApplyOrRevertResult
    {
        public IEnumerable<string> ModelMigrationsIds { get; private set; }
        public bool IsRevert { get; private set; }

        public MigrationsToApplyOrRevertResult(IEnumerable<string> modelMigrationsId, bool isRevert)
        {
            this.ModelMigrationsIds = modelMigrationsId;
            this.IsRevert = isRevert;
        }

        public static MigrationsToApplyOrRevertResult Empty
        {
            get
            {
                return new MigrationsToApplyOrRevertResult(Enumerable.Empty<string>(), false);
            }
        }
    }
}
