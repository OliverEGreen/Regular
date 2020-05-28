using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Services
{
    public static class InputValidation
    {
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
        public static List<string> IllegalRevitCharacters = new List<string>() { "/", ":", "{", "}", "[", "]", "|", ";", ">", "<", "?", "`", "~", Environment.NewLine };
        private static bool ValidateRuleName(string input)
        {
            if (input == null || input.Length < 1) return false;
            return true;
        }
        private static bool ValidateOutputParameterName(string input)
        {
            if (input == null || input.Length < 1 || IllegalRevitCharacters.Any(x => input.Contains(x))) return false;
            return true;
        }
        private static bool ValidateTargetCategoryName(string input)
        {
            if (input == null || input.Length < 1) return false;
            return true;
        }
        private static bool ValidateTargetParameterName(string input)
        {
            if (input == null || input.Length < 1) return false;
            return true;
        }
        private static bool ValidateRegexString(string input)
        {
            if (input == null || input.Length < 1) return false;
            return true;
        }
        private static bool ValidateRegexRuleParts(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            if (regexRuleParts.Count < 1) return false;
            return true;
        }
    }
}
