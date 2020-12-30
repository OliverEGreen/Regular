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
