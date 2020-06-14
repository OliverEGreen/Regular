using Regular.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Regular.Services
{
    public static class InputValidationServices
    {
        //TODO: We need to ignore non-unicode characters. In fact, we might just stick to the basic alphabet.
        //TODO: We need methods by which to test inputs given to individual RegexRuleParts

        public static int MaxInputLength = 30;

        public static string ReturnUserFeedback(string ruleNameInput, string outputParameterNameInput, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            if (!string.IsNullOrEmpty(ValidateRuleName(ruleNameInput))) return ValidateRuleName(ruleNameInput);
            if (!string.IsNullOrEmpty(ValidateOutputParameterName(outputParameterNameInput))) return ValidateOutputParameterName(outputParameterNameInput);
            if (!string.IsNullOrEmpty(ValidateRegexRuleParts(regexRuleParts))) return ValidateRegexRuleParts(regexRuleParts);
            return "";
        }

        public static List<string> IllegalRevitCharacters = new List<string> { "/", ":", "{", "}", "[", "]", "|", ";", ">", "<", "?", "`", "~", Environment.NewLine };
        public static string ValidateRuleName(string input)
        {
            return string.IsNullOrEmpty(input) ? "Rule name cannot be blank." : "";
        }
        public static string ValidateOutputParameterName(string input)
        {
            if (string.IsNullOrEmpty(input)) return "Output parameter name cannot be blank.";
            return IllegalRevitCharacters.Any(input.Contains) ? @"Output parameter name cannot contain  / : { } [ ] | ; > < ? ` ~" : "";
        }
        public static bool ValidateRegexString(string input)
        {
            return !string.IsNullOrEmpty(input);
        }
        public static string ValidateRegexRuleParts(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            // We need to build & run some validations for each kind of RegexRulePart here.
            if (regexRuleParts.Count < 1) return "Rules requires at least 1 rule part.";
            return "";
        }
    }
}
