using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regular.Models;

namespace Regular.Services
{
    public static class InputValidationServices
    {
        // TODO: We need to ignore non-unicode characters. In fact, we might just stick to the basic alphabet.
        // TODO: We need methods by which to test inputs given to individual RegexRuleParts
        
        public static List<string> IllegalRevitCharacters = new List<string> { "/", ":", "{", "}", "[", "]", "|", ";", ">", "<", "?", "`", "~", Environment.NewLine };
        public static string ValidateRuleName(RegexRule regexRule, List<RegexRule> existingRegexRules)
        {
            if (string.IsNullOrWhiteSpace(regexRule.RuleName)) return "Rule name cannot be blank";
            List<string> existingRuleNames = existingRegexRules.Select(x => x.RuleName).ToList();
            return existingRuleNames.Contains(regexRule.RuleName) ? $"Rule name '{regexRule.RuleName}' is already taken by another rule" : null;
        }
        public static string ValidateOutputParameterName(string input)
        {
            // TODO: Check this parameter name isn't already taken
            if (string.IsNullOrWhiteSpace(input)) return "Output parameter name cannot be blank.";
            return IllegalRevitCharacters.Any(input.Contains) ? @"Output parameter name cannot contain  / : { } [ ] | ; > < ? ` ~" : null;
        }

        public static string ValidateRegexRuleParts(ObservableCollection<IRegexRulePart> regexRuleParts)
        {
            // We need to build & run some validations for each kind of RegexRulePart here.
            if (regexRuleParts.Count < 1) return "Rules require at least 1 rule part.";
            return "";
        }
    }
}
