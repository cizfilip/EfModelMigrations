using EfModelMigrations.Infrastructure;
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
        public string TargetMigrationId { get; set; }

        public override void Run()
        {
            var locator = new ModelMigrationsLocator(Configuration);

            //if target not specified return all pending migrations
            if (string.IsNullOrEmpty(TargetMigrationId))
            {
                Return(new MigrationsToApplyOrRevertResult(locator.GetPendingMigrationsIds().ToList(), isRevert: false));
                return;
            }

            var appliedModelMigrations = locator.GetAppliedMigrationsIds();
            //check if fromMigration is same as last appliedMigrations
            if (string.Equals(TargetMigrationId, appliedModelMigrations.LastOrDefault(), StringComparison.Ordinal))
            {
                Return(MigrationsToApplyOrRevertResult.Empty);
                return;
            }

            //see if fromMigration is in applied migrations
            var migrationsToUnapply = appliedModelMigrations.SkipWhile(m => !string.Equals(m, TargetMigrationId, StringComparison.Ordinal));
            if (migrationsToUnapply.Any())
            {
                Return(new MigrationsToApplyOrRevertResult(migrationsToUnapply.Reverse().ToList(), isRevert: true));
                return;
            }

            //see if fromMigration is in pending migrations
            var pendingMigrations = locator.GetPendingMigrationsIds();
            if (pendingMigrations.Contains(TargetMigrationId, StringComparer.Ordinal))
            {
                //return all pending migration including from migration
                Return(new MigrationsToApplyOrRevertResult(
                    pendingMigrations.TakeWhile(m => !string.Equals(m, TargetMigrationId, StringComparison.Ordinal))
                        .Concat(new[] { TargetMigrationId })
                        .ToList(),
                    isRevert: true));
                return;
            }

            //if we reach here there is nothing to apply
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
                return new MigrationsToApplyOrRevertResult(new string[] { }, false);
            }
        }
    }
}
