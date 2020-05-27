using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public static class RegexRuleManager
    {
        //A central class with CRUD functionality to manage the document's RegexRules
        public static RegexRule CreateRegexRule()
        {
            //Must add the rule to both the static cache and ExtensibleStorage
            return null;
        }
        public static RegexRule FetchRegexRule(string documentGuid, string regexRuleGuid)
        {
            ObservableCollection<RegexRule> documentRegexRules = FetchDocRegexRules(documentGuid);
            if (documentRegexRules == null) return null;
            return documentRegexRules.Where(x => x.Guid == regexRuleGuid).FirstOrDefault();
        }
        public static ObservableCollection<RegexRule> FetchDocRegexRules(string documentGuid)
        {
            if (RegularApp.AllRegexRules.ContainsKey(documentGuid)) { return RegularApp.AllRegexRules[documentGuid]; }
            return null;
        }
        public static void UpdateRegexRule(string documentGuid, string regexRuleGuid, RegexRule newRegexRule)
        {
            //Takes a newly-generated RegexRule object and sets an existing rules values to match
            //To be used when updating an existing rule from the Rule Editor
            RegexRule existingRegexRule = FetchRegexRule(documentGuid, regexRuleGuid);
            if (existingRegexRule == null) return;

            existingRegexRule.IsCaseSensitive = newRegexRule.IsCaseSensitive;
            existingRegexRule.OutputParameterName = newRegexRule.OutputParameterName;
            existingRegexRule.RegexRuleParts = newRegexRule.RegexRuleParts;
            existingRegexRule.RegexString = newRegexRule.RegexString;
            existingRegexRule.RuleName = newRegexRule.RuleName;
            existingRegexRule.TargetCategory = newRegexRule.TargetCategory;
            existingRegexRule.TrackingParameterName = newRegexRule.TrackingParameterName;
            return;
        }
        public static void DeleteRegexRule(string documentGuid, string regexRuleGuid)
        {
            //Deletes a RegexRule from the static cache and the document's ExtensibleStorage
            RegexRule regexRule = FetchRegexRule(documentGuid, regexRuleGuid);
            if (regexRule == null) return;
            //Some tidying up needs to happen here. We need to remove from the static cache as well 
            //As from extensiblestorage
        }
    }
}
