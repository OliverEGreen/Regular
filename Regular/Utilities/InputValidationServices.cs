using System.Collections.Generic;
using System.Linq;
using Regular.Models;

namespace Regular.Utilities
{
    public static class InputValidationServices
    {
        public static string ValidateRuleName(RegexRule regexRule, List<RegexRule> existingRegexRules)
        {
            if (string.IsNullOrWhiteSpace(regexRule.RuleName)) return "Rule name cannot be blank";
            List<string> existingRuleNames = existingRegexRules.Select(x => x.RuleName).ToList();
            if(regexRule.IsStagingRule) return "";
            return existingRuleNames.Contains(regexRule.RuleName) ? $"Rule name '{regexRule.RuleName}' already exists" : null;
        }
    }
}
