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
            if (!string.IsNullOrEmpty((ValidateRuleName(ruleNameInput)))) return ValidateRuleName(ruleNameInput);
            if (ValidateOutputParameterName(outputParameterNameInput) == false) return @"Output Parameter cannot contain  / : { } [ ] | ; > < ? ` ~";
            if (regexRuleParts != null && regexRuleParts.Count < 1) return "Rules require at least 1 rule part";
            return "";
        }

        public static List<string> IllegalRevitCharacters = new List<string> { "/", ":", "{", "}", "[", "]", "|", ";", ">", "<", "?", "`", "~", Environment.NewLine };
        public static string ValidateRuleName(string input)
        {
            if (string.IsNullOrEmpty((input))) return "Rule name cannot be blank.";
            if (input.Length > MaxInputLength) return $"Rule name cannot be longer than {MaxInputLength} characters.";
            return "";
        }
        public static bool ValidateOutputParameterName(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Length <= MaxInputLength && !IllegalRevitCharacters.Any(input.Contains);
        }
        public static bool ValidateRegexString(string input)
        {
            return !string.IsNullOrEmpty(input);
        }
        public static bool ValidateRegexRuleParts(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            // We need to build & run some validations for each kind of RegexRulePart here.
            if (regexRuleParts.Count < 1) return false;
            return true;
        }
    }
}
