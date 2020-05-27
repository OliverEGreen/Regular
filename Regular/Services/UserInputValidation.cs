using Regular.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Services
{
    public static class UserInputValidation
    {
        public static bool ValidateRegexRule(RegexRule regexRule)
        {
            return  ValidateRuleName(regexRule.RuleName) &&
                    ValidateOutputParameterName(regexRule.OutputParameterName);
                    //Etc etc
        }
        private static bool ValidateRuleName(string input)
        {
            return false;
        }
        private static bool ValidateOutputParameterName(string input)
        {
            return false;
        }

    }
}
