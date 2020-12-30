using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Regular.Enums;
using Regular.Models;
// ReSharper disable All

namespace Regular.Utilities
{
    public static class RuleExecutionUtils
    {
        public static void ExecuteRegexRule(string documentGuid, UpdaterId updaterId, List<ElementId> modifiedElementIds)
        {
            RegexRule regexRule = RegularApp.RegexRuleCacheService
                .GetDocumentRules(documentGuid)
                .FirstOrDefault(x => x.UpdaterId == updaterId);
            if (regexRule == null) return;

            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            if (document == null) return;

            using (Transaction transaction = new Transaction(document, $"Executing Regular Rule: {regexRule.RuleName}"))
            {
                transaction.Start();
                
                for (int i = 0; i < modifiedElementIds.Count; i++)
                {
                    // Retrieving the modified element
                    Element element = RegularApp.DocumentCacheService
                        .GetDocument(documentGuid)
                        .GetElement(modifiedElementIds[i]);

                    BuiltInParameter builtInParameter = (BuiltInParameter) regexRule.OutputParameterObject.ParameterObjectId;
                    Parameter parameter = element.get_Parameter(builtInParameter);
                    if (parameter == null) continue;
                    parameter.Set(TestRuleValidity(regexRule, element));
                }
                
                transaction.Commit();
            }
        }

        public static int TestRuleValidity(RegexRule regexRule, Element element)
        {
            if (element == null) return 0;
            Parameter parameter = element.get_Parameter((BuiltInParameter)regexRule.TrackingParameterObject.ParameterObjectId);
            if (parameter == null || parameter.StorageType != StorageType.String) return 0;
            string regexString = regexRule.RegexString;
            if (String.IsNullOrWhiteSpace(regexString)) return 0;
            Regex regex = new Regex(regexRule.RegexString);
            return regex.IsMatch(parameter.AsString()) == true ? 1 : 0;
        }
    }
}
