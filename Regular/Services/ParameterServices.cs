using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Regular.Services
{
    public static class ParameterServices
    {
        public static void CreateProjectParameter(Document document, string parameterName, ParameterType parameterType, List<string> targetCategoryIdStrings, BuiltInParameterGroup builtInParameterGroup, bool isInstanceParameter)
        {
            // Spiderinnet's hacky method to create a project parameter, despite the Revit API's limitations on this
            // From https:// spiderinnet.typepad.com/blog/2011/05/parameter-of-revit-api-31-create-project-parameter.html
            // This creates a temporary shared parameters file, a temporary shared parameter
            // It then binds this back to the model as an InstanceBinding and deletes the temporary stuff

            //Creating the necessary categoryset to create the outputParameter

            List<ElementId> targetCategoryIds = targetCategoryIdStrings.Select(x => new ElementId(Convert.ToInt32(x))).ToList();
            List<Category> categories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).OfType<Category>().ToList();
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

                Binding binding = revitApplication.Create.NewTypeBinding(categorySet);
                if (isInstanceParameter) binding = revitApplication.Create.NewInstanceBinding(categorySet);

                BindingMap bindingMap = (new UIApplication(revitApplication)).ActiveUIDocument.Document.ParameterBindings;
                bindingMap.Insert(def, binding, builtInParameterGroup);

                transaction.Commit();
            }
        }
        
        // Does this help??
        // https://thebuildingcoder.typepad.com/blog/2017/01/schedule-parameter-and-shared-parameter-guid.html#3

        public static Parameter GetProjectParameterByName(Document document, string parameterName)
        {
            BindingMap map = document.ParameterBindings;
            DefinitionBindingMapIterator it = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                string currentParameterName = it.Key.Name;
                if (currentParameterName == parameterName) { return (Parameter)it.Current; }
            }
            return null;
        }

        public static List<Parameter> ConvertParameterSetToList(ParameterSet parameterSet)
        {
            List<Parameter> parameters = new List<Parameter>();
            foreach (Parameter parameter in parameterSet) parameters.Add(parameter);
            return parameters;
        }
    }
}
