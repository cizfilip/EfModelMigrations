using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Infrastructure.EntityFramework
{
    public sealed class EfModel
    {
        public EfModelMetadata Metadata { get; private set; }

        public EfModel(string edmx)
        {
            this.Metadata = EfModelMetadata.Load(edmx);
        }

        internal EfModel(EfModelMetadata metadata)
        {
            this.Metadata = metadata;
        }




    }
}
