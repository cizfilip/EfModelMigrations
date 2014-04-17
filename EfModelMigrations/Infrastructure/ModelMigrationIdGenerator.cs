using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure
{
    
    public class ModelMigrationIdGenerator
    {
        public const string InitialModel = "0";

        private static readonly Regex migrationIdPattern = new Regex(@"\d{15}_.+");
        private static readonly string migrationIdFormat = "yyyyMMddHHmmssf";

        public static string GenerateId(string migrationName)
        {
            //TODO: Generate timestamp like EF tema does - see source of EF
            
            string timestamp = DateTime.UtcNow.ToString(migrationIdFormat, CultureInfo.InvariantCulture);
            return (timestamp + "_" + migrationName);
        }

        public static bool IsValidId(string migrationId)
        {
            return migrationIdPattern.IsMatch(migrationId) || migrationId == InitialModel;
        }

        public static string GetNameFromId(string migrationId)
        {
            if (IsValidId(migrationId))
            {
                return migrationId.Substring(16);
            }
            return migrationId;            
        }
    }
}
