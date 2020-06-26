using Regular.Enums;
using Regular.ViewModel;

namespace Regular.Services
{
    public static class RulePartServices
    {
        public static RegexRulePart CreateRegexRulePart(RuleType ruleType)
        {
            RegexRulePart regexRulePart = new RegexRulePart(ruleType);
            return regexRulePart;
        }
    }
}
