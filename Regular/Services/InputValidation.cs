using Regular.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Services
{
    public static class InputValidation
    {
        public static bool ValidateInputs(string ruleNameInput, string targetCategoryName, string targetParameterName, string outputParameterNameInput)
        {
            // Public method to return the outputs of all individual tests
            return  ValidateRuleName(ruleNameInput) &&
                    ValidateTargetCategoryName(targetCategoryName) &&
                    ValidateTargetParameterName(targetParameterName) &&
                    ValidateOutputParameterName(outputParameterNameInput);
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

    }
}
