using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    //TODO: pridat encodovani a dekodovani resourcu
    //TODO: haze error pokud resource soubor vubec neexistuje - nemelo by nastat Enable ho vytvori ale handlovat by se to tady melo
    internal class ModelMigrationsConfigurationUpdater
    {
        private static readonly string AppliedMigrationsKey = "AppliedMigrations";
        private static readonly string LastAppliedDbMigrationKey = "LastAppliedDbMigrationId";

        string configurationResourcePath;

        public ModelMigrationsConfigurationUpdater(string configurationResourcePath)
        {
            this.configurationResourcePath = configurationResourcePath;
        }

        public void AddAppliedMigration(string modelMigrationId, string dbMigrationId)
        {
            string appliedMigrationsValue = ReadResource()[AppliedMigrationsKey];

            appliedMigrationsValue = string.IsNullOrWhiteSpace(appliedMigrationsValue) ? appliedMigrationsValue : appliedMigrationsValue + ",";
            appliedMigrationsValue = appliedMigrationsValue + modelMigrationId;

            WriteResource(appliedMigrationsValue, dbMigrationId);
        }

        //TODO: co takhle menit i last applied db migration id ?? 
        public void RemoveLastAppliedMigration(string dbMigrationId)
        {
            string oldAppliedMigrationsValue = ReadResource()[AppliedMigrationsKey];
            var splitted = oldAppliedMigrationsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string newAppliedMigrationsValue = string.Join(",", splitted.Take(splitted.Length - 1));

            WriteResource(newAppliedMigrationsValue, dbMigrationId);
        }

        private IDictionary<string, string> ReadResource()
        {
            var resource = new Dictionary<string, string>();

            using (ResXResourceReader reader = new ResXResourceReader(configurationResourcePath))
            {
                 IDictionaryEnumerator dict = reader.GetEnumerator();
                 while (dict.MoveNext())
                 {
                     resource.Add(dict.Key.ToString(), dict.Value.ToString());
                 }
            }

            //TODO: vyhazovat vyjimku pokud v resourcu chybi nejaky klic...
            return resource;
        }

        private void WriteResource(string appliedMigrationsValue, string lastAppliedDbMigrationValue)
        {
            //TODO: prepisuji se timhle klice v resourcu nebo je to prida znova...
            using (var writer = new ResXResourceWriter(configurationResourcePath))
            {
                writer.AddResource(AppliedMigrationsKey, appliedMigrationsValue);
                writer.AddResource(LastAppliedDbMigrationKey, lastAppliedDbMigrationValue);
            }
        }
    }
}
