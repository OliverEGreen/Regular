using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regular.Models;

namespace Regular.Services
{
    public class RegexRuleCacheService
    {
        // Singleton Service class handling RegexRules across all open documents
        private static RegexRuleCacheService RegexRuleCacheServiceServiceInstance { get; set; }
        private static Dictionary<string, ObservableCollection<RegexRule>> RegexRules { get; set; }

        private RegexRuleCacheService() {}
        public static RegexRuleCacheService Instance()
        {
            if (RegexRuleCacheServiceServiceInstance != null) return RegexRuleCacheServiceServiceInstance;
            RegexRuleCacheServiceServiceInstance = new RegexRuleCacheService();
            RegexRules = new Dictionary<string, ObservableCollection<RegexRule>>();
            return RegexRuleCacheServiceServiceInstance;
        }

        public void AddRule(string documentGuid, RegexRule regexRule)
        {
            if (RegexRules.ContainsKey(documentGuid))
            {
                RegexRules[documentGuid].Add(regexRule);
            }
            else
            {
                RegexRules[documentGuid] = new ObservableCollection<RegexRule> {regexRule};
            }
        }
        public void RemoveRule(string documentGuid, string regexRuleGuid)
        {
            if (!RegexRules.ContainsKey(documentGuid)) return;
            RegexRule regexRuleToDelete =
                GetDocumentRules(documentGuid)
                    .FirstOrDefault(x => x.RuleGuid == regexRuleGuid);
            if (regexRuleToDelete == null) return;
            RegexRules[documentGuid].Remove(regexRuleToDelete);
        }

        public void UpdateRule(string documentGuid, RegexRule regexRule)
        {
            if (RegexRules.ContainsKey(documentGuid))
            {
                // Attempting to find corresponding rule by matching GUID
                RegexRule sameGuidRegexRule = RegexRules[documentGuid].FirstOrDefault(x => x.RuleGuid == regexRule.RuleGuid);
                if (sameGuidRegexRule == null) return;
                // Replacing the old rule with the new at the same index
                int replacementIndex = RegexRules[documentGuid].IndexOf(sameGuidRegexRule);
                if (replacementIndex == -1) return;
                RegexRules[documentGuid][replacementIndex] = regexRule;
            }
        }

        public RegexRule GetRegexRule(string documentGuid, string ruleGuid)
        {
            if (!RegexRules.ContainsKey(documentGuid)) return null;
            return RegexRules[documentGuid].FirstOrDefault(x => x.RuleGuid == ruleGuid);
        }

        public ObservableCollection<RegexRule> GetDocumentRules(string documentGuid)
        {
            if (!RegexRules.ContainsKey(documentGuid))
            {
                RegexRules[documentGuid] = new ObservableCollection<RegexRule>();
            }
            return RegexRules[documentGuid];
        }

        public void ClearDocumentRules(string documentGuid)
        {
            if (RegexRules.ContainsKey(documentGuid))
            {
                RegexRules[documentGuid].Clear();
            }
        }
    }
}
