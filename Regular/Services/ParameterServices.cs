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
        public static void CreateProjectParameter(string documentGuid, string parameterName, List<string> targetCategoryIdStrings)
        {
            // Spiderinnet's hacky method to create a project parameter, despite the Revit API's limitations on this
            // From https:// spiderinnet.typepad.com/blog/2011/05/parameter-of-revit-api-31-create-project-parameter.html
            // This creates a temporary shared parameters file, a temporary shared parameter
            // It then binds this back to the model as an InstanceBinding and deletes the temporary stuff

            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            const BuiltInParameterGroup builtInParameterGroup = BuiltInParameterGroup.PG_IDENTITY_DATA;
            const ParameterType parameterType = ParameterType.Text;

            //Creating the necessary CategorySet to create the outputParameter

            List<ElementId> targetCategoryIds = targetCategoryIdStrings.Select(x => new ElementId(Convert.ToInt32(x))).ToList();
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
        
        public static Parameter GetParameterById(ElementId elementId)
        {
            // Impement this method
            return null;
        }

        public static BuiltInParameter GetBuiltInParameterById(int value) => (BuiltInParameter)value;
        
        public static ObservableCollection<ParameterObject> GetDefinitionsOfCategories(string documentGuid, List<ElementId> categoryIds)
        {
            // TODO: This is a cop-out right now. We need both the IDs and the names. And for 
            // Both built-in and internally-defined parameters. Only for text-type ones, though.
            // That way we can best-populate the UI list and assign the right ID to the RegexRule's TrackingParameterId property.
            
            ObservableCollection<ParameterObject> parameterElements = new ObservableCollection<ParameterObject>();
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            List<ElementId> parameterIds = ParameterFilterUtilities.GetFilterableParametersInCommon(document, categoryIds).ToList();
            foreach (ElementId parameterId in parameterIds)
            {
                //
            }
            return parameterElements;
        }

        public static List<Parameter> ConvertParameterSetToList(ParameterSet parameterSet)
        {
            List<Parameter> parameters = new List<Parameter>();
            foreach (Parameter parameter in parameterSet) parameters.Add(parameter);
            return parameters;
        }
    }
}
