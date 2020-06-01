using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regular.Services
{
    public static class RegexAssembly
    {
        private static readonly List<string> SpecialCharacters = new List<string>() { @".", @"\", @"*", @"+", @"?", @"|", @"(", @")", @"[", @"]", @"^", @"{", @"}" };
        private static string GetRegexPartFromRuleType(RegexRulePart regexRulePart)
        {
            // TODO: Need to handle case sensitive and optional booleans.
            // For optional we can append ? to each returned string.
            // For non case-sensitive (i.e. case match) we can append the (?i) modifier after the string
            switch (regexRulePart.RuleType)
            {
                case RuleTypes.FreeText:
                    return SanitizeWord(regexRulePart.RawUserInputValue);
                case RuleTypes.SelectionSet:
                    // We'll need to break these up somehow
                    return "Test";
                case RuleTypes.AnyLetter:
                    if (regexRulePart.IsCaseSensitive) { }
                    // How do we handle case-sensitive?
                    return @"[a-zA-Z]";
                case RuleTypes.AnyDigit:
                    return @"\d";
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
            // TODO: We need to finalize the regex string. 
            // Are we using a re.match or a re.search? contain $ or ^? Or we could dynamically switch out the method
            return regexString;
        }
    }
}
