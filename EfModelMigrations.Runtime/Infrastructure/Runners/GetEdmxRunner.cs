using EfModelMigrations.Infrastructure.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EfModelMigrations.Runtime.Infrastructure.Runners
{
    [Serializable]
    internal class GetEdmxRunner : BaseRunner
    {      
        public override void Run()
        {
            Return(new EdmxModelExtractor().GetEdmxModelAsString(DbContext));
        }

        
    }
}
