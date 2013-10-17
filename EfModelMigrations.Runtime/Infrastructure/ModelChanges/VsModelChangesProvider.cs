using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Runtime.Extensions;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers;
using EfModelMigrations.Runtime.Properties;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsModelChangesProvider : IModelChangesProvider, IDbContextChangesProvider
    {
        private Project modelProject;
        //TODO: asi by stacilo aby ClassCodeModel mela property FullName (ale musim nejak zaridit aby mela modelnamespace)
        private string modelNamespace;
        private string dbContextFullName;
        private ICodeGenerator codeGenerator;
        private CodeClassFinder classFinder;


        public VsModelChangesProvider(Project modelProject, 
            string modelNamespace, 
            string dbContextFullName,
            ICodeGenerator codeGenerator)
        {
            this.modelProject = modelProject;
            this.modelNamespace = modelNamespace;
            this.dbContextFullName = dbContextFullName;
            this.codeGenerator = codeGenerator;
            this.classFinder = new CodeClassFinder(modelProject);
        }


        public IDbContextChangesProvider ChangeDbContext
        {
            get { return this; }
        }

        #region IModelChangesProvider Implementation
        

        public void CreateEmptyClass(ClassCodeModel classModel)
        {
            //TODO: ujistit se za pouzivam validni C# identifikatory - pomoci metody CodeModel.IsValidID
            string classContent = codeGenerator.GenerateEmptyClass(classModel);
            string filePath = Path.Combine(GetConventionPathFromNamespace(modelNamespace), classModel.Name + codeGenerator.GetFileExtensions());

            try
            {
                modelProject.AddContentToProject(filePath, classContent);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToCreateClass, classModel.Name), e);
            }

        }

        //TODO: resit partial tridy (pres CodeClass2.Parts)
        //TODO: metoda Remove odebira z projektu ale file zustava fyzicky na disku - slo by vyuzit pro revert, Delete metoda smaze i file
        public void RemoveClass(ClassCodeModel classModel)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, classModel.Name);

            try
            {
                var projectItem = codeClass.ProjectItem;
                projectItem.Delete();
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveClass, classModel.Name), e);
            }
        }


        public void AddPropertyToClass(ClassCodeModel classModel, PropertyCodeModel propertyModel)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, classModel.Name);

            AddPropertyToClassInternal(codeClass, 
                codeGenerator.GenerateProperty(propertyModel), 
                e => new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToAddProperty, 
                    propertyModel.Name, 
                    classModel.Name), e)
                );
        }

        public void RemovePropertyFromClass(ClassCodeModel classModel, PropertyCodeModel propertyModel)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, classModel.Name);

            CodeProperty2 codeProperty = FindProperty(codeClass, propertyModel.Name);

            try
            {
                codeClass.RemoveMember(codeProperty);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveProperty, propertyModel.Name, classModel.Name), e);
            }
        }

        public void RenameClass(ClassCodeModel classModel, string newName)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, classModel.Name);

            CodeElement2 classElement = codeClass as CodeElement2;

            try
            {
                classElement.RenameSymbol(newName);
                // Rename file with source code of class - not good what about partial classes etc...
                // vyhazuje vyjímku když soubor s novým názvem existuje !!
                classElement.ProjectItem.Name = newName + ".cs";
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRenameClass, classModel.Name), e);
            }
        }

        public void RenameProperty(ClassCodeModel classModel, PropertyCodeModel propertyModel, string newName)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(modelNamespace, classModel.Name);
            CodeElement2 property = FindProperty(codeClass, propertyModel.Name) as CodeElement2;
            try
            {
                property.RenameSymbol(newName);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRenameProperty, propertyModel.Name, classModel.Name), e);
            }
        }
        #endregion 

        #region IDbContextChangesProvider Implementation

        public void AddDbSetPropertyForClass(ClassCodeModel classForAddProperty)
        {
            CodeClass2 contextClass = GetDbContextCodeClass();

            AddPropertyToClassInternal(contextClass,
                codeGenerator.GenerateDbSetProperty(classForAddProperty),
                e => new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToAddDbSetProperty,
                    classForAddProperty.Name), e)
                );
        }

        public void RemoveDbSetPropertyForClass(ClassCodeModel classForRemoveProperty)
        {
            CodeClass2 contextClass = GetDbContextCodeClass();

            CodeProperty2 property = FindPropertyOnDbContext(contextClass, classForRemoveProperty);
            
            try
            {
                contextClass.RemoveMember(property);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveDbSetProperty, classForRemoveProperty.Name), e);
            }
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

        private CodeProperty2 FindPropertyOnDbContext(CodeClass2 ctxClass, ClassCodeModel classForRemoveProperty)
        {
            try
            {
                CodeProperty2 property = ctxClass.Children.OfType<CodeProperty2>()
                    .Where(p => IsDbSetPropertyForClass(p.Type as CodeTypeRef2, classForRemoveProperty))
                    .Single();

                return property;
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToFindDbSetProperty, classForRemoveProperty.Name), e);
            }
        }

        private bool IsDbSetPropertyForClass(CodeTypeRef2 codeTypeRef, ClassCodeModel classForRemoveProperty)
        {
            if(codeTypeRef == null || !codeTypeRef.IsGeneric)
                return false;

            string fullTypeName = codeTypeRef.AsFullName;

            string genericTypeName = null;
            Match match = Regex.Match(fullTypeName, @"[^<]+<(.+)>$", RegexOptions.None);
            if (match.Success)
            {
                genericTypeName = match.Groups[1].Value;
            }

            string fullClassName = modelNamespace + "." + classForRemoveProperty.Name;

            if (fullClassName.EqualsOrdinal(genericTypeName))
                return true;

            return false;
        }

        #endregion


        
    }
}
