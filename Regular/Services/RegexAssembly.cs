using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Services
{
    public static class RegexAssembly
    {
        private static Dictionary<RuleTypes, string> RulePartTranslations = new Dictionary<RuleTypes, string>()
        {
            { RuleTypes.AnyCharacter, "" },
            { RuleTypes.AnyFromSet, "" },
            { RuleTypes.AnyLetter, "" },
            { RuleTypes.AnyNumber, "" },
            { RuleTypes.Anything, "" },
            { RuleTypes.Dot, "" },
            { RuleTypes.Hyphen, "" },
            { RuleTypes.SpecificCharacter, "" },
            { RuleTypes.SpecificLetter, "" },
            { RuleTypes.SpecificNumber, "" },
            { RuleTypes.Underscore, "" },
        };
        private static string GetRegexPartFromRuleType(RegexRulePart regexRulePart)
        {
            switch (regexRulePart.RuleType)
            {
                case RuleTypes.AnyCharacter:
                    return @"\w";
                case RuleTypes.AnyFromSet:
                    // We'll need to break these up somehow
                    return "Test";
                case RuleTypes.AnyLetter:
                    return @"[a-zA-Z]";
                case RuleTypes.AnyNumber:
                    return @"\d";
                case RuleTypes.Anything:
                    return @".";
                case RuleTypes.Dot:
                    return @"\.";
                case RuleTypes.Hyphen:
                    return @"\-";
                case RuleTypes.SpecificCharacter:
                    return $@"[{regexRulePart.RawUserInputValue}]";
                case RuleTypes.SpecificLetter:
                    return $@"[{regexRulePart.RawUserInputValue}]";
                case RuleTypes.SpecificNumber:
                    return $@"[{regexRulePart.RawUserInputValue}]";
                case RuleTypes.Underscore:
                    return @"_";
                default:
                    return null;
            }
        }
        public static string AssembleRegexString(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            string regexString = "";
            foreach(RegexRulePart regexRulePart in regexRuleParts) { regexString += GetRegexPartFromRuleType(regexRulePart); }
            return regexString.Replace(@"\\", @"\");
        }
    }
}
