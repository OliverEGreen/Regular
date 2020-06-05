using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regular.Services
{
    public static class InputValidationServices
    {
        //TODO: We need to ignore non-unicode characters. In fact, we might just stick to the basic alphabet.
        //TODO: We need methods by which to test inputs given to individual RegexRuleParts
        public static bool ValidateInputs(string ruleNameInput, string targetCategoryName, string targetParameterName, string outputParameterNameInput, string regexStringInput, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            // Public method to return the outputs of all individual tests
            return  ValidateRuleName(ruleNameInput) &&
                    ValidateTargetCategoryName(targetCategoryName) &&
                    ValidateTargetParameterName(targetParameterName) &&
                    ValidateOutputParameterName(outputParameterNameInput) &&
                    ValidateRegexString(regexStringInput) &&
                    ValidateRegexRuleParts(regexRuleParts);
        }

        public static string ReturnUserFeedback(string ruleNameInput, string outputParameterNameInput, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            if (ValidateRuleName(ruleNameInput) == false) return "Rule Name cannot exceed 50 characters";
            else if (ValidateOutputParameterName(outputParameterNameInput) == false) return @"Output Parameter cannot contain  / : { } [ ] | ; > < ? ` ~";
            else if (regexRuleParts != null && regexRuleParts.Count < 1) return "Rules require at least 1 rule part";
            else return null;
        }

        public static List<string> IllegalRevitCharacters = new List<string>() { "/", ":", "{", "}", "[", "]", "|", ";", ">", "<", "?", "`", "~", Environment.NewLine };
        public static bool ValidateRuleName(string input)
        {
            if (input == null || input.Length < 1 || input.Length > 50) return false;
            return true;
        }
        public static bool ValidateOutputParameterName(string input)
        {
            if (input == null || input.Length < 1 || IllegalRevitCharacters.Any(x => input.Contains(x))) return false;
            return true;
        }
        public static bool ValidateTargetCategoryName(string input)
        {
            if (input == null || input.Length < 1) return false;
            return true;
        }
        public static bool ValidateTargetParameterName(string input)
        {
            if (input == null || input.Length < 1) return false;
            return true;
        }
        public static bool ValidateRegexString(string input)
        {
            if (input == null || input.Length < 1) return false;
            return true;
        }
        public static bool ValidateRegexRuleParts(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            if (regexRuleParts.Count < 1) return false;
            return true;
        }
    }
}
