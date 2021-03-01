using System;
using Regular.Enums;
using Regular.Models.RegexRuleParts;

namespace Regular.Models
{
    public class RegexRulePart
    {
        // Takes a RuleType enum value and returns a new RegexRulePart implementing IRegexRulePart
        public static IRegexRulePart Create(RuleType ruleType)
        {
            switch (ruleType)
            {
                case RuleType.AnyAlphanumeric:
                    return new AnyAlphanumeric();
                case RuleType.AnyDigit:
                    return new AnyDigit();
                case RuleType.AnyLetter:
                    return new AnyLetter();
                case RuleType.CustomText:
                    return new CustomText();
                case RuleType.OptionSet:
                    return new OptionSet();
                case RuleType.FullStop:
                    return new FullStop();
                case RuleType.Hyphen:
                    return new Hyphen();
                case RuleType.Underscore:
                    return new Underscore();
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
            }
        }
    }
}
