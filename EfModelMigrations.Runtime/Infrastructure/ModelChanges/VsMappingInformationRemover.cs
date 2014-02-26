using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Runtime.Properties;
using EfModelMigrations.Extensions;
using EnvDTE80;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsMappingInformationRemover : IMappingInformationRemover
    {
        private HistoryTrackerWrapper historyTracker;
        private string modelNamespace;
        private string dbContextFullName;
        private CodeClassFinder classFinder;

        public VsMappingInformationRemover(HistoryTrackerWrapper historyTracker, string modelNamespace, string dbContextFullName, CodeClassFinder classFinder)
        {
            this.historyTracker = historyTracker;
            this.modelNamespace = modelNamespace;
            this.dbContextFullName = dbContextFullName;
            this.classFinder = classFinder;
        }

        public virtual void Remove(IMappingInformation mappingInformation)
        {
            try
            {
                dynamic mappingInfo = mappingInformation;
                RemoveMapping(mappingInfo);
            }
            catch (RuntimeBinderException e)
            {
                //TODO: string do resourcu
                throw new ModelMigrationsException(string.Format("Cannot remove mapping information {0}. Remover implementation is missing.", mappingInformation.GetType().Name), e);
            }
        }


        //TODO: dodelat removing mapovacich informaci pro asociace
        protected virtual void RemoveMapping(AssociationInfo mappingInformation)
        {

        }



    }
}
