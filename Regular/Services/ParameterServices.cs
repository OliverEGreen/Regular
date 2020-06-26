using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Regular.ViewModel;

namespace Regular.Services
{
    public static class ParameterServices
    {
        public static void CreateProjectParameter(string documentGuid, string parameterName, ObservableCollection<CategoryObject> targetCategoryObjects)
        {
            // Spiderinnet's hacky method to create a project parameter, despite the Revit API's limitations on this
            // From https:// spiderinnet.typepad.com/blog/2011/05/parameter-of-revit-api-31-create-project-parameter.html
            // This creates a temporary shared parameters file, a temporary shared parameter
            // It then binds this back to the model as an InstanceBinding and deletes the temporary stuff

            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            const BuiltInParameterGroup builtInParameterGroup = BuiltInParameterGroup.PG_IDENTITY_DATA;
            const ParameterType parameterType = ParameterType.YesNo;

            //Creating the necessary CategorySet to create the outputParameter

            List<ElementId> targetCategoryIds = targetCategoryObjects.Where(x => x.IsChecked).Select(x => new ElementId(x.CategoryObjectId)).ToList();
            List<Category> categories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).ToList();
            CategorySet categorySet = CategoryServices.GetCategorySetFromList(document, categories);
                        
            using (Transaction transaction = new Transaction(document, $"Regular - Creating New Project Parameter {parameterName}")) 
            {
                transaction.Start();

                Application revitApplication = RegularApp.RevitApplication;

                string oriFile = revitApplication.SharedParametersFilename;
                string tempFile = Path.GetTempFileName() + ".txt";
                using (File.Create(tempFile)) { }
                revitApplication.SharedParametersFilename = tempFile;
                ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterName, parameterType);
                ExternalDefinition def = revitApplication.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;

                revitApplication.SharedParametersFilename = oriFile;
                File.Delete(tempFile);

                Binding binding = revitApplication.Create.NewInstanceBinding(categorySet);
                BindingMap bindingMap = new UIApplication(revitApplication).ActiveUIDocument.Document.ParameterBindings;
                bindingMap.Insert(def, binding, builtInParameterGroup);

                transaction.Commit();
            }
        }
        
        public static string GetParameterName(Document document, ElementId parameterId)
        {
            if (parameterId.IntegerValue < 0) return LabelUtils.GetLabelFor((BuiltInParameter)parameterId.IntegerValue);
            return ((ParameterElement)document.GetElement(parameterId)).GetDefinition().Name;
        }

        public static ObservableCollection<ParameterObject> GetParametersOfCategories(string documentGuid, ObservableCollection<CategoryObject> categoryObjects)
        {
            // We'll return this ObservableCollection of ParameterObjects straight for UI consumption
            ObservableCollection<ParameterObject> parameterObjects = new ObservableCollection<ParameterObject>();
            
            
            List<ElementId> categoryIds = categoryObjects
                .Where(x => x.IsChecked)
                .Select(x => Convert.ToInt32(x.CategoryObjectId))
                .Select(x => new ElementId(x))
                .ToList();
            
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            List<ElementId> parameterIds = ParameterFilterUtilities.GetFilterableParametersInCommon(document, categoryIds).ToList();
            
            foreach (ElementId parameterId in parameterIds)
            {
                string parameterName = GetParameterName(document, parameterId);
                parameterObjects.Add(new ParameterObject { ParameterObjectId = parameterId.IntegerValue, ParameterObjectName = parameterName });
            }
            return new ObservableCollection<ParameterObject>(parameterObjects.OrderBy(x => x.ParameterObjectName));
        }

        public static List<Parameter> ConvertParameterSetToList(ParameterSet parameterSet)
        {
            List<Parameter> parameters = new List<Parameter>();
            foreach (Parameter parameter in parameterSet) parameters.Add(parameter);
            return parameters;
        }
    }
}
