using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Regular.Models;
// ReSharper disable All

namespace Regular.Utilities
{
    public static class RuleExecutionUtils
    {
        public static void TriggerExecuteRegexRule(string documentGuid, UpdaterId updaterId, List<ElementId> modifiedElementIds)
        {
            // Executes a single RegexRule on a limited set of elements
            // Used when user has modified elements in the document and set off the DMU trigger
            
            RegexRule regexRule = RegularApp.RegexRuleCacheService
                .GetDocumentRules(documentGuid)
                .FirstOrDefault(x => x.RegularUpdater.GetUpdaterId() == updaterId);
            if (regexRule == null) return;

            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            if (document == null) return;

            for (int i = 0; i < modifiedElementIds.Count; i++)
            {
                // Retrieving the modified element
                Element element = RegularApp.DocumentCacheService
                    .GetDocument(documentGuid)
                    .GetElement(modifiedElementIds[i]);

                BuiltInParameter builtInParameter = (BuiltInParameter)regexRule.OutputParameterObject.ParameterObjectId;
                Parameter parameter = element.get_Parameter(builtInParameter);
                if (parameter == null) continue;
                parameter.Set(TestRuleValidity(regexRule, element));
            }
        }

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
                .ToElements()
                .ToList();

            if (targetedElements == null || targetedElements.Count < 1) return;

            ElementId trackingParameterId = new ElementId(regexRule.TrackingParameterObject.ParameterObjectId);

            using(Transaction transaction = new Transaction(document, $"Executing Rule: {regexRule.RuleName}"))
            {
                transaction.Start();

                for (int i = 0; i < targetedElements.Count; i++)
                {
                    BuiltInParameter builtInParameter = (BuiltInParameter) regexRule.OutputParameterObject.ParameterObjectId;
                    Parameter parameter = targetedElements[i].get_Parameter(builtInParameter);
                    if (parameter == null) continue;
                    parameter.Set(TestRuleValidity(regexRule, targetedElements[i]));
                }

                transaction.Commit();
            }
        }

        public static void ExecuteDocumentRegexRules(string documentGuid)
        {
            // Runs validation on all regex rules in a document
            // Used for when opening a file

            List<RegexRule> documentRegexRules = RegularApp.RegexRuleCacheService.GetDocumentRules(documentGuid).ToList();
            if (documentRegexRules.Count < 1) return;

            for (int i = 0; i < documentRegexRules.Count; i++)
            {
                ExecuteRegexRule(documentGuid, documentRegexRules[i]);
            }
        }

        private static int TestRuleValidity(RegexRule regexRule, Element element)
        {
            // Tests whether a saved rule's regular expression matches the element's parameter value
            if (element == null) return 0;
            Parameter parameter = element.get_Parameter((BuiltInParameter)regexRule.TrackingParameterObject.ParameterObjectId);
            if (parameter == null || parameter.StorageType != StorageType.String) return 0;
            string parameterValue = parameter.AsString();
            if (string.IsNullOrWhiteSpace(parameterValue)) return 0;
            string regexString = regexRule.RegexString;
            if (string.IsNullOrWhiteSpace(regexString)) return 0;
            Regex regex = new Regex(regexRule.RegexString);
            return regex.IsMatch(parameterValue) == true ? 1 : 0;
        }
    }
}
