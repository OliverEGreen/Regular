using System.Collections.Generic;

namespace Regular.Enums
{
    public static class EnumDicts
    {
        public static Dictionary<string, RuleType> RulesTypeDict = new Dictionary<string, RuleType>
        {
            { "Any Alphanumeric", RuleType.AnyAlphanumeric },
            { "Any Digit", RuleType.AnyDigit },
            { "Any Letter", RuleType.AnyLetter },
            { "Custom Text", RuleType.CustomText },
            { "Option Set", RuleType.OptionSet },
            { "Full Stop", RuleType.FullStop },
            { "Hyphen", RuleType.Hyphen },
            { "Underscore", RuleType.Underscore },
        };

        public static Dictionary<string, MatchType> MatchTypesDict = new Dictionary<string, MatchType>
        {
            { "Exact Match", MatchType.ExactMatch },
            { "Match At Beginning", MatchType.MatchAtBeginning },
            { "Match At End", MatchType.MatchAtEnd },
            { "Match Anywhere", MatchType.MatchAnywhere },
        };
    }
}
