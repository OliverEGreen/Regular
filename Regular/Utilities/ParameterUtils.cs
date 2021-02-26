using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Regular.Models;

namespace Regular.Utilities
{
    public static class ParameterUtils
    {
        public static void CreateProjectParameter(string documentGuid, RegexRule regexRule)
        {
            // Spiderinnet's hacky method to create a project parameter, despite the Revit API's limitations on this
            // From https:// spiderinnet.typepad.com/blog/2011/05/parameter-of-revit-api-31-create-project-parameter.html
            // This creates a temporary shared parameters file, a temporary shared parameter
            // It then binds this back to the model as an InstanceBinding and deletes the temporary stuff

            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            const BuiltInParameterGroup builtInParameterGroup = BuiltInParameterGroup.PG_IDENTITY_DATA;
            const ParameterType parameterType = ParameterType.YesNo;

            //Creating the necessary CategorySet to create the outputParameter

            List<ElementId> targetCategoryIds = regexRule.TargetCategoryObjects.Where(x => x.IsChecked).Select(x => new ElementId(x.CategoryObjectId)).ToList();
            List<Category> categories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).ToList();
            CategorySet categorySet = CategoryUtils.ConvertListToCategorySet(categories);
            ExternalDefinition definition;

            using (Transaction transaction = new Transaction(document, $"Regular - Creating New Project Parameter { regexRule.OutputParameterObject.ParameterObjectName }")) 
            {
                transaction.Start();

                Application revitApplication = RegularApp.RevitApplication;

                string oriFile = revitApplication.SharedParametersFilename;
                string tempFile = Path.GetTempFileName() + ".txt";
                using (File.Create(tempFile)) { }
                revitApplication.SharedParametersFilename = tempFile;
                ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(regexRule.OutputParameterObject.ParameterObjectName, parameterType);
                definition = revitApplication.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
                
                revitApplication.SharedParametersFilename = oriFile;
                File.Delete(tempFile);

                Binding binding = revitApplication.Create.NewInstanceBinding(categorySet);
                BindingMap bindingMap = new UIApplication(revitApplication).ActiveUIDocument.Document.ParameterBindings;
                bindingMap.Insert(definition, binding, builtInParameterGroup);
                
                transaction.Commit();

                if (definition != null)
                {
                    regexRule.OutputParameterObject.ParameterObjectId =
                        GetProjectParameterByName(documentGuid, definition.Name).Id.IntegerValue;
                }
            }
            
        }
        
        public static string GetParameterName(Document document, ElementId parameterId)
        {
            if (parameterId.IntegerValue < 0) return LabelUtils.GetLabelFor((BuiltInParameter)parameterId.IntegerValue);
            return ((ParameterElement)document.GetElement(parameterId)).GetDefinition().Name;
        }

        public static ParameterElement GetProjectParameterByName(string documentGuid, string parameterName)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            List<ParameterElement> parameterElements = new FilteredElementCollector(document)
                .OfClass(typeof(ParameterElement))
                .OfType<ParameterElement>().ToList();
            if (parameterElements.Count < 1) return null;
            return parameterElements.FirstOrDefault(x => x.Name == parameterName);
        }

        public static ObservableCollection<ParameterObject> GetParametersOfCategories(string documentGuid, ObservableCollection<CategoryObject> categoryObjects)
        {
            // We'll return this ObservableCollection of ParameterObjects straight for UI consumption
            ObservableCollection<ParameterObject> parameterObjects = new ObservableCollection<ParameterObject>();
            
            List<ElementId> categoryIds = categoryObjects
                .Where(x => x.IsChecked)
                .Select(x => x.CategoryObjectId)
                .Select(x => new ElementId(x))
                .ToList();
            
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            // Trying to only retrieve parameter of string type
            List<ElementId> parameterIds = ParameterFilterUtilities.GetFilterableParametersInCommon(document, categoryIds).ToList();
            
            foreach (ElementId parameterId in parameterIds)
            {
                parameterObjects.Add(
                    new ParameterObject
                    {
                        ParameterObjectId = parameterId.IntegerValue,
                        ParameterObjectName = GetParameterName(document, parameterId)
                    });
            }
            return new ObservableCollection<ParameterObject>(parameterObjects.OrderBy(x => x.ParameterObjectName));
        }
    }
}
