using Regular.Enums;
using Regular.Model;
using Regular.ViewModel;

namespace Regular.Services
{
    public static class RulePartServices
    {
        public static RegexRulePart CreateRegexRulePart(RuleTypes ruleType)
        {
            RegexRulePart regexRulePart = new RegexRulePart(ruleType);
            
            return regexRulePart;
        }
    }
}
