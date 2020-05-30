using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regular.Services
{
    public static class RegexAssembly
    {
        private static List<string> SpecialCharacters = new List<string>() { @".", @"\", @"*", @"+", @"?", @"|", @"(", @")", @"[", @"]", @"^", @"{", @"}" };
        private static string GetRegexPartFromRuleType(RegexRulePart regexRulePart)
        {
            switch (regexRulePart.RuleType)
            {
                case RuleTypes.AnyCharacter:
                    return SanitizeCharacter(regexRulePart.RawUserInputValue);
                case RuleTypes.AnyFromSet:
                    // We'll need to break these up somehow
                    return "Test";
                case RuleTypes.AnyLetter:
                    if (regexRulePart.IsCaseSensitive) { }
                    // How do we handle case-sensitive?
                    return @"[a-zA-Z]";
                case RuleTypes.AnyDigit:
                    return @"\d";
                case RuleTypes.SpecificCharacter:
                    return $@"[{regexRulePart.RawUserInputValue}]";
                case RuleTypes.SpecificWord:
                    return SanitizeWord(regexRulePart.RawUserInputValue);
                default:
                    return null;
            }
        }
        private static string SanitizeCharacter(string character)
        {
            if (SpecialCharacters.Contains(character)) return $@"\{character}";
            return character;
        }
        private static string SanitizeWord(string word)
        {
            string outputString = "";
            foreach(char character in word)
            {
                outputString += SanitizeCharacter(character.ToString());
            }
            return outputString;
        }
        public static string AssembleRegexString(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            string regexString = "";
            foreach(RegexRulePart regexRulePart in regexRuleParts) { regexString += GetRegexPartFromRuleType(regexRulePart); }
            return regexString.Replace(@"\\", @"\");
        }
    }
}
