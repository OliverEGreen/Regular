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

        public static string ReturnUserFeedback(string ruleNameInput, string outputParameterNameInput, ObservableCollection<IRegexRulePart> regexRuleParts)
        {
            if (!string.IsNullOrEmpty(ValidateRuleName(ruleNameInput))) return ValidateRuleName(ruleNameInput);
            if (!string.IsNullOrEmpty(ValidateOutputParameterName(outputParameterNameInput))) return ValidateOutputParameterName(outputParameterNameInput);
            if (!string.IsNullOrEmpty(ValidateRegexRuleParts(regexRuleParts))) return ValidateRegexRuleParts(regexRuleParts);
            return "";
        }

        public static List<string> IllegalRevitCharacters = new List<string> { "/", ":", "{", "}", "[", "]", "|", ";", ">", "<", "?", "`", "~", Environment.NewLine };
        public static string ValidateRuleName(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? "Rule name cannot be blank." : null;
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
