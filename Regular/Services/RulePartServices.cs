using Regular.ViewModels;

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
                    regexRulePart.RuleTypeDisplayText = "Any Digit";
                    regexRulePart.IsCaseSensitive = false;
                    regexRulePart.IsEditable = false;
                    regexRulePart.EditButtonDisplayText = "0-9";
                    break;
                case RuleTypes.AnyLetter:
                    regexRulePart.RuleTypeDisplayText = "Any Letter";
                    regexRulePart.IsCaseSensitive = true;
                    regexRulePart.IsEditable = true;
                    regexRulePart.EditButtonDisplayText = "A-Z";
                    break;
                case RuleTypes.FreeText:
                    regexRulePart.RuleTypeDisplayText = "Free Text";
                    regexRulePart.IsCaseSensitive = true;
                    regexRulePart.IsEditable = true;
                    regexRulePart.EditButtonDisplayText = "...";
                    break;
                case RuleTypes.SelectionSet:
                    regexRulePart.RuleTypeDisplayText = "Selection Set";
                    regexRulePart.IsCaseSensitive = true;
                    regexRulePart.IsEditable = true;
                    regexRulePart.EditButtonDisplayText = "...";
                    break;
            }
            return regexRulePart;
        }
    }
}
