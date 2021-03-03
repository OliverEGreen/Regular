using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Regular.Models;

namespace Regular.Utilities
{
    public static class InputValidationServices
    {
        // TODO: We need to ignore non-unicode characters. In fact, we might just stick to the basic alphabet.
        
        public static List<string> IllegalRevitCharacters = new List<string> { "/", ":", "{", "}", "[", "]", "|", ";", ">", "<", "?", "`", "~", Environment.NewLine };
        public static string ValidateRuleName(RegexRule regexRule, List<RegexRule> existingRegexRules)
        {
            if (string.IsNullOrWhiteSpace(regexRule.RuleName)) return "Rule name cannot be blank";
            List<string> existingRuleNames = existingRegexRules.Select(x => x.RuleName).ToList();
            if(regexRule.IsStagingRule) return "";
            return existingRuleNames.Contains(regexRule.RuleName) ? $"Rule name '{regexRule.RuleName}' already exists" : null;
        }
        public static string ValidateOutputParameterName(Document document, RegexRule stagingRule)
        {
            string input = stagingRule.OutputParameterObject.ParameterObjectName;

            if (string.IsNullOrWhiteSpace(input)) return "Output parameter name cannot be blank.";
            
            List<ParameterElement> parameterElements = new FilteredElementCollector(document)
                .OfClass(typeof(ParameterElement))
                .OfType<ParameterElement>()
                .ToList();

            List<string> existingParameterNames = parameterElements.Select(x => x.GetDefinition().Name).ToList();
            if(existingParameterNames.Contains(input) && !stagingRule.IsStagingRule) return $"Parameter name {input} is already in use.";
            return IllegalRevitCharacters.Any(input.Contains) ? @"Output parameter name cannot contain  / : { } [ ] | ; > < ? ` ~" : null;
        }
    }
}
