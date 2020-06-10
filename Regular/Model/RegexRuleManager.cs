using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Regular.ViewModel;

namespace Regular.Model
{
    public static class RegexRuleManager
    {
        // A central class with CRUD functionality to manage the document's RegexRules
        public static RegexRule SaveRegexRule(string documentGuid, RegexRule regexRule)
        {
            RegexRules.AllRegexRules[documentGuid].Add(regexRule);
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
            if (RegexRules.AllRegexRules.ContainsKey(documentGuid)) { return RegexRules.AllRegexRules[documentGuid]; }
            return null;
        }
        public static List<string> GetDocumentRegexRuleGuids(string documentGuid)
        {
            return GetDocumentRegexRules(documentGuid).Select(x => x.Guid).ToList();
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
            existingRegexRule.Name = newRegexRule.Name;
            existingRegexRule.TargetCategoryIds = newRegexRule.TargetCategoryIds;
            existingRegexRule.TrackingParameterName = newRegexRule.TrackingParameterName;
        }
        public static void DeleteRegexRule(string documentGuid, string regexRuleGuid)
        {
            // Deletes a RegexRule from the document's static cache
            if (RegexRules.AllRegexRules.ContainsKey(documentGuid))
            {
                ObservableCollection<RegexRule> documentRegexRules = GetDocumentRegexRules(documentGuid);
                RegexRule regexRule = documentRegexRules.Where(x => x.Guid == regexRuleGuid).FirstOrDefault();
                if (regexRule != null) documentRegexRules.Remove(regexRule);
            }
        }
    }
}
