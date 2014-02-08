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
        private HistoryTracker historyTracker;
        private string modelNamespace;
        private string dbContextFullName;
        private CodeClassFinder classFinder;

        public VsMappingInformationRemover(HistoryTracker historyTracker, string modelNamespace, string dbContextFullName, CodeClassFinder classFinder)
        {
            this.historyTracker = historyTracker;
            this.modelNamespace = modelNamespace;
            this.dbContextFullName = dbContextFullName;
            this.classFinder = classFinder;
        }

        public void Remove(IMappingInformation mappingInformation)
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

        private void RemoveMapping(DbSetPropertyInfo mappingInfo)
        {
            CodeClass2 contextClass = GetDbContextCodeClass();

            historyTracker.MarkItemModified(contextClass.ProjectItem);

            CodeProperty2 property = FindPropertyOnDbContext(contextClass, mappingInfo.ClassName);

            try
            {
                contextClass.RemoveMember(property);
            }
            catch (Exception e)
            {
                //TODO: přeorganizovat resource stringy pro add/remove db set property
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveDbSetProperty, mappingInfo.ClassName), e);
            }
        }

        #region Private methods

        private CodeClass2 GetDbContextCodeClass()
        {
            return classFinder.FindCodeClassFromFullName(dbContextFullName);
        }

        private CodeProperty2 FindPropertyOnDbContext(CodeClass2 ctxClass, string classNameForRemoveProperty)
        {
            try
            {
                CodeProperty2 property = ctxClass.Children.OfType<CodeProperty2>()
                    .Where(p => IsDbSetPropertyForClass(p.Type as CodeTypeRef2, classNameForRemoveProperty))
                    .Single();

                return property;
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToFindDbSetProperty, classNameForRemoveProperty), e);
            }
        }

        private bool IsDbSetPropertyForClass(CodeTypeRef2 codeTypeRef, string classNameForRemoveProperty)
        {
            if (codeTypeRef == null || !codeTypeRef.IsGeneric)
                return false;

            string fullTypeName = codeTypeRef.AsFullName;

            string genericTypeName = null;
            Match match = Regex.Match(fullTypeName, @"[^<]+<(.+)>$", RegexOptions.None);
            if (match.Success)
            {
                genericTypeName = match.Groups[1].Value;
            }

            string fullClassName = modelNamespace + "." + classNameForRemoveProperty;

            if (fullClassName.EqualsOrdinal(genericTypeName))
                return true;

            return false;
        }

        #endregion
    }
}
