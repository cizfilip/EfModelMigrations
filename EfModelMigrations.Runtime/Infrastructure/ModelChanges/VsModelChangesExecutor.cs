using EfModelMigrations.Exceptions;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Operations;
using EfModelMigrations.Operations.Mapping;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers;
using EfModelMigrations.Runtime.Properties;
using EnvDTE;
using EnvDTE80;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsModelChangesExecutor : IModelChangesExecutor
    {
        private Project modelProject;
        //TODO: asi by stacilo aby ClassCodeModel mela property FullName (ale musim nejak zaridit aby mela modelnamespace)
        private string modelNamespace;
        private string dbContextFullName;
        private ICodeGenerator codeGenerator;
        private CodeClassFinder classFinder;
        private IMappingInformationRemover mappingRemover;


        public VsModelChangesExecutor(Project modelProject,
            string modelNamespace,
            string dbContextFullName,
            ICodeGenerator codeGenerator)
        {
            this.modelProject = modelProject;
            this.modelNamespace = modelNamespace;
            this.dbContextFullName = dbContextFullName;
            this.codeGenerator = codeGenerator;
            this.classFinder = new CodeClassFinder(modelProject);
            this.mappingRemover = new VsMappingInformationRemover(modelNamespace, dbContextFullName, classFinder);
        }


        public void Execute(IEnumerable<IModelChangeOperation> operations)
        {

            foreach (dynamic operation in operations)
            {
                try
                {
                    ExecuteOperation(operation);
                }
                catch (RuntimeBinderException e)
                {
                    //TODO: string do resourcu
                    throw new ModelMigrationsException(string.Format("Cannot execute model change operation {0}. Executor implementation is missing.", operation.GetType().Name), e);
                }
            }

        }


        protected void ExecuteOperation(CreateEmptyClassOperation operation)
        {
            //TODO: ujistit se za pouzivam validni C# identifikatory - pomoci metody CodeModel.IsValidID
            //TODO: zde propagovat defaultni nastaveni pro tridy z configurace...
            string classContent = codeGenerator.GenerateEmptyClass(operation.Name, modelNamespace, CodeModelVisibility.Public, null, null);

            string filePath = Path.Combine(GetConventionPathFromNamespace(modelNamespace), operation.Name + codeGenerator.GetFileExtensions());

            try
            {
                modelProject.AddContentToProject(filePath, classContent);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToCreateClass, operation.Name), e);
            }
        }

        //TODO: resit partial tridy (pres CodeClass2.Parts)
        //TODO: metoda Remove odebira z projektu ale file zustava fyzicky na disku - slo by vyuzit pro revert, Delete metoda smaze i file
        //TODO: remove class vymaze vse, v pripade chyby a vraceni zpet nejsme schopni obnovit cely zdrojak tak jak byl 
        protected void ExecuteOperation(RemoveClassOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.Name);

            try
            {
                var projectItem = codeClass.ProjectItem;
                projectItem.Delete();
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveClass, operation.Name), e);
            }
        }

        protected void ExecuteOperation(AddPropertyToClassOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.ClassName);

            AddPropertyToClassInternal(codeClass,
                codeGenerator.GenerateProperty(operation.Model),
                e => new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToAddProperty,
                    operation.Model.Name,
                    operation.ClassName), e)
                );
        }

        protected void ExecuteOperation(RemovePropertyFromClassOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.ClassName);

            CodeProperty2 codeProperty = FindProperty(codeClass, operation.Name);

            try
            {
                codeClass.RemoveMember(codeProperty);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveProperty, operation.Name, operation.ClassName), e);
            }
        }

        protected void ExecuteOperation(RenameClassOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.OldName);

            CodeElement2 classElement = codeClass as CodeElement2;

            try
            {
                classElement.RenameSymbol(operation.NewName);
                // Rename file with source code of class - not good what about partial classes etc...
                // vyhazuje vyjímku když soubor s novým názvem existuje !!
                classElement.ProjectItem.Name = operation.NewName + ".cs";
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRenameClass, operation.OldName), e);
            }
        }

        protected void ExecuteOperation(RenamePropertyOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.ClassName);
            CodeElement2 property = FindProperty(codeClass, operation.OldName) as CodeElement2;
            try
            {
                property.RenameSymbol(operation.NewName);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRenameProperty, operation.OldName, operation.ClassName), e);
            }
        }

        #region Mapping Informations

        protected void ExecuteOperation(AddMappingInformationOperation operation)
        {
            var generatedInfo = codeGenerator.MappingGenerator.Generate(operation.MappingInformation);
            switch (generatedInfo.Type)
            {
                case MappingInformationType.DbContextProperty:
                    CodeClass2 contextClass = GetDbContextCodeClass();
                    AddPropertyToClassInternal(contextClass, generatedInfo.Value,
                        e => new ModelMigrationsException(
                            string.Format(Resources.VsCodeModel_FailedToAddDbSetProperty,
                            (operation.MappingInformation as DbSetPropertyInfo).ClassName)
                            , e)
                        );
                    break;
                case MappingInformationType.EntityTypeConfiguration:

                    break;
                case MappingInformationType.CodeAttribute:
                    throw new NotImplementedException();
                default:
                    //TODO: string do resourcu
                    throw new ModelMigrationsException("Unknown generated mapping information type.");
            }
        }

        protected void ExecuteOperation(RemoveMappingInformationOperation operation)
        {
            mappingRemover.Remove(operation.MappingInformation);
        }
                

        #endregion

        #region Helper private methods

        private void AddPropertyToClassInternal(CodeClass2 codeClass, string propertyString, Func<Exception, ModelMigrationsException> exceptionFactory)
        {
            try
            {
                CodeVariable tempVar = codeClass.AddVariable("__EFMODELMIGRATIONS_TEMP_VAR__", vsCMTypeRef.vsCMTypeRefInt, -1); // -1 znamena na konec
                var startPoint = tempVar.GetStartPoint();
                startPoint.CreateEditPoint().ReplaceText(tempVar.GetEndPoint(), propertyString, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
            }
            catch (Exception e)
            {
                throw exceptionFactory(e);
            }
        }

        private CodeProperty2 FindProperty(CodeClass2 codeClass, string propertyName)
        {
            try
            {
                return (CodeProperty2)codeClass.Members.Item(propertyName);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToFindProperty, propertyName, codeClass.Name), e);
            }
        }

        private string GetConventionPathFromNamespace(string @namespace)
        {
            string modelProjectRootNamespace = modelProject.GetRootNamespace();
            string namespaceWithoutRoot = @namespace.Replace(modelProjectRootNamespace, "");

            var splitted = namespaceWithoutRoot.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string path = "";
            foreach (var dir in splitted)
            {
                path = Path.Combine(path, dir);
            }
            return path;
        }

        private CodeClass2 GetDbContextCodeClass()
        {
            return classFinder.FindCodeClassFromFullName(dbContextFullName);
        }

        

        

        #endregion




    }
}
