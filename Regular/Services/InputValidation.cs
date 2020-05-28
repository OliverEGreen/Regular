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
        private static bool ValidateRuleName(string input)
        {
            // Logic goes here
            return false;
        }
        private static bool ValidateOutputParameterName(string input)
        {
            // Logic goes here does a parameter with this name exist already?
            return false;
        }
        private static bool ValidateTargetCategoryName(string input)
        {
            // Logic goes here does this category exist
            return false;
        }
        private static bool ValidateTargetParameterName(string input)
        {
            // Logic goes here does this parameter exist?
            return false;
        }

    }
}
