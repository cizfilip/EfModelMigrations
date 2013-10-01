using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
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
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.ModelChanges
{
    internal class VsModelChangesProvider : IModelChangesProvider
    {
        private Project modelProject;
        private string modelNamespace;
        private ICodeGenerator codeGenerator;
        private CodeClassFinder classFinder;

        public VsModelChangesProvider(Project modelProject, string modelNamespace, ICodeGenerator codeGenerator)
        {
            this.modelProject = modelProject;
            this.modelNamespace = modelNamespace;
            this.codeGenerator = codeGenerator;
            this.classFinder = new CodeClassFinder(modelProject, modelNamespace);
        }

        public void CreateEmptyClass(ClassCodeModel classModel)
        {
            //TODO: ujistit se za pouzivam validni C# identifikatory - pomoci metody CodeModel.IsValidID
            classModel.Namespace = modelNamespace;

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
            CodeClass2 codeClass = classFinder.FindCodeClass(classModel.Name);

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
            CodeClass2 codeClass = classFinder.FindCodeClass(classModel.Name);

            try
            {
                CodeVariable tempVar = codeClass.AddVariable("__EFMODELMIGRATIONS_TEMP_VAR__", vsCMTypeRef.vsCMTypeRefInt, -1); // -1 znamena na konec
                string propertyString = codeGenerator.GenerateProperty(propertyModel);

                var startPoint = tempVar.GetStartPoint();
                startPoint.CreateEditPoint().ReplaceText(tempVar.GetEndPoint(), propertyString, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToAddProperty, propertyModel.Name, classModel.Name), e);
            }
        }

        public void RemovePropertyFromClass(ClassCodeModel classModel, PropertyCodeModel propertyModel)
        {
            CodeClass2 codeClass = classFinder.FindCodeClass(classModel.Name);

            try
            {
                var codeProperty = codeClass.Members.Item(propertyModel.Name);

                codeClass.RemoveMember(codeProperty);
            }
            catch (Exception e)
            {
                throw new ModelMigrationsException(string.Format(Resources.VsCodeModel_FailedToRemoveProperty, propertyModel.Name, classModel.Name), e);
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
    }
}
