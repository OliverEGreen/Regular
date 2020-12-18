using System;
using System.ComponentModel;
using System.Windows;
using Regular.Enums;
using Regular.Models.RegexRuleParts;

namespace Regular.Models
{
    public static class RegexRulePart
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
                case RuleType.FreeText:
                    return new FreeText();
                case RuleType.SelectionSet:
                    return new SelectionSet();
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
            }
        }
    }
}
