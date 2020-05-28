using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Regular.Models;
using Regular.Services;
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
        // A central class with CRUD functionality to manage the document's RegexRules
        public static RegexRule CreateRegexRule(string ruleName, string targetCategoryName, string trackingParameterName, string outputParameterName)
        {
            return new RegexRule(ruleName, targetCategoryName, trackingParameterName, outputParameterName);
            
            
                        

            /*
            foreach (RegexRulePart regexRulePart in selectedRegexRuleParts)
            {
                RegexRule.RegexString += GetRegexPartFromRuleType(regexRulePart); // Something!! We build the string as we close the editor 
            }
            
            */
        }
        public static RegexRule GetRegexRule(string documentGuid, string regexRuleGuid)
        {
            ObservableCollection<RegexRule> documentRegexRules = GetDocRegexRules(documentGuid);
            if (documentRegexRules == null) return null;
            return documentRegexRules.Where(x => x.Guid == regexRuleGuid).FirstOrDefault();
        }
        public static ObservableCollection<RegexRule> GetDocRegexRules(string documentGuid)
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

            existingRegexRule.IsCaseSensitive = newRegexRule.IsCaseSensitive;
            existingRegexRule.OutputParameterName = newRegexRule.OutputParameterName;
            existingRegexRule.RegexRuleParts = newRegexRule.RegexRuleParts;
            existingRegexRule.RegexString = newRegexRule.RegexString;
            existingRegexRule.RuleName = newRegexRule.RuleName;
            existingRegexRule.TargetCategoryName = newRegexRule.TargetCategoryName;
            existingRegexRule.TrackingParameterName = newRegexRule.TrackingParameterName;
            return;
        }
        public static void DeleteRegexRule(string documentGuid, string regexRuleGuid)
        {
            // Deletes a RegexRule from the static cache and the document's ExtensibleStorage
            RegexRule regexRule = GetRegexRule(documentGuid, regexRuleGuid);
            if (regexRule == null) return;
            // Some tidying up needs to happen here. We need to remove from the static cache as well 
            // As from extensiblestorage
        }
    }
}
