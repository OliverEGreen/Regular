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
            const ParameterType parameterType = ParameterType.Text;

            //Creating the necessary CategorySet to create the outputParameter
            List<ElementId> targetCategoryIds = regexRule.TargetCategoryObjects.Where(x => x.IsChecked).Select(x => new ElementId(x.CategoryObjectId)).ToList();
            List<Category> categories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).ToList();
            CategorySet categorySet = CategoryUtils.ConvertListToCategorySet(categories);

            using (Transaction transaction = new Transaction(document, $"Regular - Creating New Project Parameter { regexRule.OutputParameterObject.ParameterObjectName }")) 
            {
                transaction.Start();

                Application application = RegularApp.RevitApplication;

                string oriFile = application.SharedParametersFilename;
                string tempFile = Path.GetTempFileName() + ".txt";
                using (File.Create(tempFile)) { }
                application.SharedParametersFilename = tempFile;
                ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(regexRule.OutputParameterObject.ParameterObjectName, parameterType);
                ExternalDefinition externalDefinition = application.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
            
                application.SharedParametersFilename = oriFile;
                File.Delete(tempFile);

                Binding binding = application.Create.NewTypeBinding(categorySet);
                binding = application.Create.NewInstanceBinding(categorySet);
            
                SharedParameterElement sharedParameterElement = SharedParameterElement.Lookup(document, externalDefinition.GUID );
                sharedParameterElement?.GetDefinition().SetAllowVaryBetweenGroups(document, true);

                BindingMap bindingMap = (new UIApplication(application)).ActiveUIDocument.Document.ParameterBindings;
                bindingMap.Insert(externalDefinition, binding, builtInParameterGroup);

                transaction.Commit();

                ParameterElement parameterElement = GetProjectParameterByName(documentGuid, externalDefinition.Name);
                regexRule.OutputParameterObject.ParameterObjectId = parameterElement.Id.IntegerValue;
            }
        }

        public static void ForceOutputParameterToVaryBetweenGroups(string documentGuid, RegexRule regexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            //Modifying Parameter so it varies according to group instance.
            using (Transaction transaction = new Transaction(document, "Setting Parameter to Vary By Group Instance"))
            {
                transaction.Start();
                BindingMap map = document.ParameterBindings;
                DefinitionBindingMapIterator it = map.ForwardIterator();
                it.Reset();
                while (it.MoveNext())
                {
                    Definition definition = it.Key;
                    if (definition.Name != regexRule.OutputParameterObject.ParameterObjectName) continue;
                    InternalDefinition internalDef = definition as InternalDefinition;
                    internalDef?.SetAllowVaryBetweenGroups(document, true);
                }
                transaction.Commit();
            }
        }
        
        private static void UpdateTargetCategoryIds(string documentGuid, RegexRule newRegexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            ParameterElement parameterElement = GetProjectParameterByName(documentGuid, newRegexRule.OutputParameterObject.ParameterObjectName);
            
            // Helper method to get the instance binding of a ParameterElement object from the document's BindingMap
            InstanceBinding GetParameterInstanceBinding()
            {
                InternalDefinition parameterElementDefinition = parameterElement.GetDefinition();
                BindingMap bindingMap = document.ParameterBindings;
                DefinitionBindingMapIterator iterator = bindingMap.ForwardIterator();
                iterator.Reset();

                while (iterator.MoveNext())
                {
                    Definition iteratorKey = iterator.Key;
                    if (iteratorKey.Name == parameterElementDefinition.Name) return (InstanceBinding)iterator.Current;
                }
                return null;
            }
            
            InstanceBinding instanceBinding = GetParameterInstanceBinding();
            InternalDefinition internalDefinition = parameterElement.GetDefinition();
            
            List<ElementId> targetCategoryIds = newRegexRule.TargetCategoryObjects
                .Where(x => x.IsChecked)
                .Select(x => new ElementId(x.CategoryObjectId))
                .ToList();

            List<Category> categories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).ToList();
            
            instanceBinding.Categories.Clear();
            foreach (Category category in categories) instanceBinding.Categories.Insert(category);

            // Apparently this is needed to force the document to accept the new bindings
            document.ParameterBindings.ReInsert(internalDefinition, instanceBinding);
        }

        public static void UpdateProjectParameter(string documentGuid, RegexRule existingRegexRule, RegexRule newRegexRule)
        {
            bool CompareOutputParameters()
            {
                // Compares the TargetCategoryIds of two RegexRules

                List<int> existingTargetCategoryIds = existingRegexRule.TargetCategoryObjects
                    .Where(x => x.IsChecked)
                    .Select(x => x.CategoryObjectId)
                    .ToList();

                List<int> newTargetCategoryIds = newRegexRule.TargetCategoryObjects
                    .Where(x => x.IsChecked)
                    .Select(x => x.CategoryObjectId)
                    .ToList();
            
                var removedIds = existingTargetCategoryIds.Except(newTargetCategoryIds).ToList();
                var addedIds = newTargetCategoryIds.Except(existingTargetCategoryIds).ToList();
                bool targetCategoryIdsMatch = !removedIds.Any() && !addedIds.Any();

                return targetCategoryIdsMatch;
            }
            
            // If no changes have been made, we can simply return
            if (CompareOutputParameters()) return;
            
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            ParameterElement parameterElement = GetProjectParameterByName(documentGuid, existingRegexRule.OutputParameterObject.ParameterObjectName);
            if (parameterElement == null) return;
            
            using (Transaction transaction = new Transaction(document, $"Regular - Updating Parameter { newRegexRule.OutputParameterObject.ParameterObjectName }")) 
            {
                transaction.Start();
                // Updating the TargetCategoryIds
                UpdateTargetCategoryIds(documentGuid, newRegexRule);
                transaction.Commit();
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
            
            return parameterElements.Count < 1 ? null : parameterElements.FirstOrDefault(x => x.Name == parameterName);
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
