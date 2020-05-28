using Regular.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Regular
{
    public static class RegexRuleManager
    {
        // A central class with CRUD functionality to manage the document's RegexRules
        public static RegexRule AddRegexRule(string documentGuid, string ruleName, string targetCategoryName, string trackingParameterName, string outputParameterName, string regexString, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            RegexRule regexRule = new RegexRule(ruleName, targetCategoryName, trackingParameterName, outputParameterName, regexString, regexRuleParts);
            RegularApp.AllRegexRules[documentGuid].Add(regexRule);
            return regexRule;
        }
        public static RegexRule GetRegexRule(string documentGuid, string regexRuleGuid)
        {
            ObservableCollection<RegexRule> documentRegexRules = GetDocumentRegexRules(documentGuid);
            if (documentRegexRules == null) return null;
            return documentRegexRules.Where(x => x.Guid == regexRuleGuid).FirstOrDefault();
        }
        public static ObservableCollection<RegexRule> GetDocumentRegexRules(string documentGuid)
        {
            if (RegularApp.AllRegexRules.ContainsKey(documentGuid)) { return RegularApp.AllRegexRules[documentGuid]; }
            return null;
        }
        public static void UpdateRegexRule(string documentGuid, string regexRuleGuid, RegexRule newRegexRule)
        {
            // Takes a newly-generated RegexRule object and sets an existing rules values to match
            // To be used when updating an existing rule from the Rule Editor
            RegexRule existingRegexRule = GetRegexRule(documentGuid, regexRuleGuid);
            if (existingRegexRule == null) return;

            existingRegexRule.OutputParameterName = newRegexRule.OutputParameterName;
            existingRegexRule.RegexRuleParts = newRegexRule.RegexRuleParts;
            existingRegexRule.RegexString = newRegexRule.RegexString;
            existingRegexRule.RuleName = newRegexRule.RuleName;
            existingRegexRule.TargetCategoryName = newRegexRule.TargetCategoryName;
            existingRegexRule.TrackingParameterName = newRegexRule.TrackingParameterName;
        }
        public static void DeleteRegexRule(string documentGuid, string regexRuleGuid)
        {
            // Deletes a RegexRule from the document's static cache
            if (RegularApp.AllRegexRules.ContainsKey(documentGuid))
            {
                ObservableCollection<RegexRule> documentRegexRules = GetDocumentRegexRules(documentGuid);
                RegexRule regexRule = documentRegexRules.Where(x => x.Guid == regexRuleGuid).FirstOrDefault();
                if (regexRule != null) documentRegexRules.Remove(regexRule);
            }
        }
    }
}
