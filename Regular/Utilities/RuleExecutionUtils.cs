using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Regular.Models;

namespace Regular.Utilities
{
    public static class RuleExecutionUtils
    {
        public static void ExecuteRegexRule(string documentGuid, RegexRule regexRule)
        {
            // Executes a single RegexRule in a document on all elements it affects
            // Used when creating or updating a rule
            if (regexRule == null) return;

            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(
                regexRule.TargetCategoryObjects
                .Where(x => x.IsChecked)
                .Select(x => Category.GetCategory(document, new ElementId(x.CategoryObjectId)))
                .Select(CategoryUtils.GetBuiltInCategoryFromCategory)
                .ToList());

            List<Element> targetedElements = new FilteredElementCollector(document)
                .WhereElementIsNotElementType()
                .WherePasses(elementMulticategoryFilter)
                .Where(x => x.GroupId == ElementId.InvalidElementId)
                .ToList();

            if (targetedElements.Count < 1) return;
            
            ElementId trackingParameterId = new ElementId(regexRule.TrackingParameterObject.ParameterObjectId);

            for (int i = 0; i < targetedElements.Count; i++)
            {
                // TODO: Report Window!
                string report = TestRuleValidity(regexRule, targetedElements[i]);
            }
        }
        
        private static string TestRuleValidity(RegexRule regexRule, Element element)
        {
            // Tests whether a saved rule's regular expression matches the element's parameter value
            // Ideally would be a Boolean value, but we can only programatically create Project Instance Parameters as strings ¯\_(ツ)_/¯
            Parameter parameter = element?.get_Parameter((BuiltInParameter)regexRule.TrackingParameterObject.ParameterObjectId);
            if (parameter == null || parameter.StorageType != StorageType.String) return "Invalid";
            string parameterValue = parameter.AsString();
            if (string.IsNullOrWhiteSpace(parameterValue)) return "Invalid";
            string regexString = regexRule.RegexString;
            if (string.IsNullOrWhiteSpace(regexString)) return "Invalid";
            Regex regex = new Regex(regexRule.RegexString);
            return regex.IsMatch(parameterValue) == true ? "Valid" : "Invalid";
        }
    }
}
