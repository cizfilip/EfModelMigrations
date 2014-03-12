﻿using EfModelMigrations.Exceptions;
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
        private HistoryTracker historyTracker;
        private Project modelProject;
        //TODO: asi by stacilo aby ClassCodeModel mela property FullName (ale musim nejak zaridit aby mela modelnamespace)
        private string modelNamespace;
        private string dbContextFullName;
        private ICodeGenerator codeGenerator;
        private CodeClassFinder classFinder;
        private CSharpRegexMappingGenerator regexMappingGenerator;


        public VsModelChangesExecutor(HistoryTracker historyTracker,
            Project modelProject,
            string modelNamespace,
            string dbContextFullName,
            ICodeGenerator codeGenerator)
        {
            this.historyTracker = historyTracker;
            this.modelProject = modelProject;
            this.modelNamespace = modelNamespace;
            this.dbContextFullName = dbContextFullName;
            this.codeGenerator = codeGenerator;
            this.classFinder = new CodeClassFinder(modelProject);
            this.regexMappingGenerator = new CSharpRegexMappingGenerator();
        }


        public virtual void Execute(IEnumerable<IModelChangeOperation> operations)
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


        protected virtual void ExecuteOperation(CreateEmptyClassOperation operation)
        {
            //TODO: ujistit se za pouzivam validni C# identifikatory - pomoci metody CodeModel.IsValidID
            //TODO: zde propagovat defaultni nastaveni pro tridy z configurace...
            string fileFullPath = Path.Combine(modelProject.GetProjectDir(), GetConventionPathFromNamespace(modelNamespace), operation.Name + codeGenerator.GetFileExtensions());
            historyTracker.MarkItemAdded(fileFullPath);

            string classContent = codeGenerator.GenerateEmptyClass(operation.Name, modelNamespace, operation.Visibility, null, null);
            try
            {
                var newProjectItem = modelProject.AddContentToProjectFromAbsolutePath(fileFullPath, classContent);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToCreateClass, operation.Name), e);
            }
        }

        //TODO: resit partial tridy (pres CodeClass2.Parts)
        //TODO: metoda Remove odebira z projektu ale file zustava fyzicky na disku - slo by vyuzit pro revert, Delete metoda smaze i file
        //TODO: remove class vymaze vse, v pripade chyby a vraceni zpet nejsme schopni obnovit cely zdrojak tak jak byl 
        protected virtual void ExecuteOperation(RemoveClassOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.Name);

            try
            {
                var projectItem = codeClass.ProjectItem;
                historyTracker.MarkItemDeleted(projectItem);
                projectItem.Delete();
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveClass, operation.Name), e);
            }
        }

        protected virtual void ExecuteOperation(AddPropertyToClassOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.ClassName);

            historyTracker.MarkItemModified(codeClass.ProjectItem);

            string propertyName;
            string propertyString = codeGenerator.GenerateProperty(operation.Model, out propertyName);

            AddPropertyToClassInternal(codeClass,
                propertyName,
                propertyString,
                e => new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToAddProperty,
                    operation.Model.Name,
                    operation.ClassName), e)
                );
        }

        protected virtual void ExecuteOperation(RemovePropertyFromClassOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.ClassName);

            historyTracker.MarkItemModified(codeClass.ProjectItem);

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

        protected virtual void ExecuteOperation(RenameClassOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.OldName);

            historyTracker.MarkItemModified(codeClass.ProjectItem);

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

        protected virtual void ExecuteOperation(RenamePropertyOperation operation)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, operation.ClassName);

            historyTracker.MarkItemModified(codeClass.ProjectItem);

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


        protected virtual void ExecuteOperation(MovePropertyOperation operation)
        {
            CodeClass2 fromCodeClass = classFinder.FindCodeClass(modelNamespace, operation.FromClassName);
            CodeClass2 toCodeClass = classFinder.FindCodeClass(modelNamespace, operation.ToClassName);

            historyTracker.MarkItemModified(fromCodeClass.ProjectItem);
            historyTracker.MarkItemModified(toCodeClass.ProjectItem);

            try
            {
                CodeProperty2 property = FindProperty(fromCodeClass, operation.Name);
                var endPoint = property.GetEndPoint();
                var startEditPoint = property.GetStartPoint(vsCMPart.vsCMPartWholeWithAttributes).CreateEditPoint();
                string propertyString = startEditPoint.GetText(endPoint);
                startEditPoint.Delete(endPoint);

                AddPropertyToClassInternal(toCodeClass,
                    operation.Name,
                    propertyString,
                    e => new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToAddProperty,
                        operation.Name,
                        operation.ToClassName), e)
                    );
                
            }
            catch (ModelMigrationsException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException("Failed to move property.", e); //TODO: string do resourcu
            }

            
        }

        #region Mapping Informations

        protected virtual void ExecuteOperation(AddMappingInformationOperation operation)
        {
                        
            var mappingGenerator = codeGenerator.MappingGenerator;

            var generatedInfo = mappingGenerator.GenerateFluentApiCall(operation.MappingInformation.BuildEfFluentApiCallChain());
            var prefixForOnModelCreating = mappingGenerator.GetPrefixForOnModelCreatingUse(generatedInfo.TargetType);
            

            CodeClass2 contextClass = GetDbContextCodeClass();
            historyTracker.MarkItemModified(contextClass.ProjectItem);

            CodeFunction2 onModelCreatingMethod = FindOnModelCreatingMethod(contextClass);


            try
            {
                var editPoint = onModelCreatingMethod.GetEndPoint(vsCMPart.vsCMPartBody).CreateEditPoint();

                editPoint.Insert(string.Concat(prefixForOnModelCreating, generatedInfo.Content));
                editPoint.Insert(Environment.NewLine);

                //Format inserted mapping
                onModelCreatingMethod.StartPoint.CreateEditPoint().SmartFormat(onModelCreatingMethod.EndPoint);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToAddMappingInformation, operation.GetType().Name), e);
            }
            
        }

        protected virtual void ExecuteOperation(RemoveMappingInformationOperation operation)
        {
            CodeClass2 contextClass = GetDbContextCodeClass();
            historyTracker.MarkItemModified(contextClass.ProjectItem);

            CodeFunction2 onModelCreatingMethod = FindOnModelCreatingMethod(contextClass);

            string oldCode = GetMethodCode(onModelCreatingMethod);

            //TODO: dodelat remove mapovani i v jinych castech (EntityTypeConfiguration ... attributy u trid...)
            var generatedInfo = regexMappingGenerator.GenerateFluentApiCall(operation.MappingInformation.BuildEfFluentApiCallChain());
            var prefixForOnModelCreating = regexMappingGenerator.GetPrefixForOnModelCreatingUse(generatedInfo.TargetType);

            var matches = Regex.Matches(oldCode, string.Concat(prefixForOnModelCreating, generatedInfo.Content), RegexOptions.None);

            if(matches.Count > 1)
            {
                throw new ModelMigrationsException("More than one mapping information found in OnModelCreating"); //TODO: string do resourcu
            }

            if (matches.Count == 1)
            {
                var match = matches[0];
                string newCode = oldCode.Remove(match.Index, match.Length);
                SetMethodCode(onModelCreatingMethod, newCode);
            }
        }

        protected virtual void ExecuteOperation(AddDbSetPropertyOperation operation)
        {
            CodeClass2 contextClass = GetDbContextCodeClass();

            historyTracker.MarkItemModified(contextClass.ProjectItem);

            string dbSetPropertyName;
            string propertyString = codeGenerator.GenerateDbSetProperty(operation.ClassName, out dbSetPropertyName);

            AddPropertyToClassInternal(contextClass, dbSetPropertyName, propertyString,
                e => new ModelMigrationsException(
                    string.Format(Resources.VsCodeModel_FailedToAddDbSetProperty,
                    operation.ClassName)
                    , e)
                );
        }

        protected virtual void ExecuteOperation(RemoveDbSetPropertyOperation operation)
        {
            CodeClass2 contextClass = GetDbContextCodeClass();

            historyTracker.MarkItemModified(contextClass.ProjectItem);

            CodeProperty2 property = FindPropertyOnDbContext(contextClass, operation.ClassName);

            try
            {
                contextClass.RemoveMember(property);
            }
            catch (Exception e)
            {
                //TODO: přeorganizovat resource stringy pro add/remove db set property
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveDbSetProperty, operation.ClassName), e);
            }
        }


        #endregion

        #region Helper private methods

        private void AddPropertyToClassInternal(CodeClass2 codeClass, string propertyName, string propertyString, Func<Exception, ModelMigrationsException> exceptionFactory)
        {
            try
            {
                CodeVariable tempVar = codeClass.AddVariable("__EFMODELMIGRATIONS_TEMP_VAR__", vsCMTypeRef.vsCMTypeRefInt, -1); // -1 znamena na konec
                var startPoint = tempVar.GetStartPoint();
                startPoint.CreateEditPoint().ReplaceText(tempVar.GetEndPoint(), propertyString, EnvDteExtensions.AllvsEPReplaceTextOptionsFlags());

                //Format inserted property
                var property = FindProperty(codeClass, propertyName);
                property.StartPoint.CreateEditPoint().SmartFormat(property.EndPoint);
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

        private CodeFunction2 FindMethod(CodeClass2 codeClass, string methodName)
        {
            try
            {
                return (CodeFunction2)codeClass.Members.Item(methodName);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToFindMethod, methodName, codeClass.Name), e);
            }
        }

        private CodeFunction2 FindOnModelCreatingMethod(CodeClass2 contextClass = null)
        {
            if(contextClass == null)
            {
                contextClass = GetDbContextCodeClass();
            }

            return FindMethod(contextClass, "OnModelCreating");
        }

        private string GetMethodCode(CodeFunction2 method)
        {
            try
            {
                var startPoint = method.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                var endPoint = method.GetEndPoint(vsCMPart.vsCMPartBody);

                return startPoint.GetText(endPoint);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format("Cannot retrieve method code from method {0}", method.Name), e); //TODO: string do resourcu
            }
        }

        private void SetMethodCode(CodeFunction2 method, string code)
        {
            try
            {
                var startPoint = method.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                var endPoint = method.GetEndPoint(vsCMPart.vsCMPartBody);

                startPoint.ReplaceText(endPoint, code, EnvDteExtensions.AllvsEPReplaceTextOptionsFlags());
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format("Cannot update method code in method {0}", method.Name), e); //TODO: string do resourcu
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
