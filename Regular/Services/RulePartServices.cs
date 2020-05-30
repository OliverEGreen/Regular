using Regular.Models;

namespace Regular.Services
{
    public static class RulePartServices
    {
        public static RegexRulePart CreateRegexRulePart(RuleTypes ruleType)
        {
            RegexRulePart regexRulePart = new RegexRulePart(ruleType);
            switch (ruleType)
            {
                case RuleTypes.AnyCharacter:
                    regexRulePart.RuleTypeDisplayString = "Any Character";
                    break;
                case RuleTypes.AnyDigit:
                    regexRulePart.RuleTypeDisplayString = "Any Digit(0-9)";
                    break;
                case RuleTypes.AnyLetter:
                    regexRulePart.RuleTypeDisplayString = "Any Letter (A-Z)";
                    break;
                case RuleTypes.AnyFromSet:
                    regexRulePart.RequiresUserInput = true;
                    regexRulePart.RuleTypeDisplayString = "Any From Set";
                    break;
                case RuleTypes.SpecificCharacter:
                    regexRulePart.RequiresUserInput = true;
                    regexRulePart.RuleTypeDisplayString = "Specific Character";
                    break;
                case RuleTypes.SpecificWord:
                    regexRulePart.RequiresUserInput = true;
                    regexRulePart.RuleTypeDisplayString = "Specific Word";
                    break;
            }
            return regexRulePart;
        }
    }
}
