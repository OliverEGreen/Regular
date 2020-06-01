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
                case RuleTypes.AnyDigit:
                    regexRulePart.RuleTypeDisplayString = "Any Digit";
                    break;
                case RuleTypes.AnyLetter:
                    regexRulePart.RuleTypeDisplayString = "Any Letter";
                    break;
                case RuleTypes.FreeText:
                    regexRulePart.RequiresUserInput = true;
                    regexRulePart.RuleTypeDisplayString = "Free Text";
                    break;
                case RuleTypes.SelectionSet:
                    regexRulePart.RequiresUserInput = true;
                    regexRulePart.RuleTypeDisplayString = "Selection Set";
                    break;
            }
            return regexRulePart;
        }
    }
}
