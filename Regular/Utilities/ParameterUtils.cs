using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Regular.Models;

namespace Regular.Utilities
{
    public static class ParameterUtils
    {
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
                parameterObjects.Add
                (
                    new ParameterObject
                    {
                        ParameterObjectId = parameterId.IntegerValue,
                        ParameterObjectName = GetParameterName(document, parameterId)
                    }
                );
            }
            return new ObservableCollection<ParameterObject>(parameterObjects.OrderBy(x => x.ParameterObjectName));
        }

        public static string GetTrackingParameterValue(string documentGuid, string ruleGuid, Element element)
        {
            RegexRule regexRule = RegularApp.RegexRuleCacheService.GetRegexRule(documentGuid, ruleGuid);
            if (regexRule == null) return null;
            
            Parameter parameter = element?.get_Parameter((BuiltInParameter)regexRule.TrackingParameterObject.ParameterObjectId);
            if (parameter == null || parameter.StorageType != StorageType.String) return null;
            string parameterValue = parameter.AsString();
            return parameterValue;
        }

        public static Parameter GetParameterById(Document document, Element element, int parameterIdIntegerValue)
        {
            // Creating a list of all the element's Parameter IDs
            List<int> elementParameterIds = SetUtils.ConvertParameterSetToList(element.Parameters)
                .Select(x => x.Id.IntegerValue)
                .ToList();
            List<int> elementTypeParameterIds = new List<int>();

            // If the element has a Type, we'll want to keep track of its Parameter IDs
            ElementType elementType = document.GetElement(element.GetTypeId()) as ElementType;
            if (elementType != null)
            {
                elementTypeParameterIds.AddRange
                (
                    SetUtils.ConvertParameterSetToList(elementType.Parameters)
                    .Select(x => x.Id.IntegerValue)
                    .ToList()
                );
            }

            if (parameterIdIntegerValue < 0)
            {
                if (elementParameterIds.Contains(parameterIdIntegerValue)) return element.get_Parameter((BuiltInParameter) parameterIdIntegerValue);
                return elementTypeParameterIds.Contains(parameterIdIntegerValue) ? elementType.get_Parameter((BuiltInParameter) parameterIdIntegerValue) : null;
            }

            Definition definition = ((ParameterElement) document.GetElement(new ElementId(parameterIdIntegerValue)))
                .GetDefinition();
            
            if (elementParameterIds.Contains(parameterIdIntegerValue)) return element.get_Parameter(definition);
            return elementTypeParameterIds.Contains(parameterIdIntegerValue) ? elementType.get_Parameter(definition) : null;
        }
    }
}
