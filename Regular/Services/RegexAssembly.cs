using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Regular.Enums;
using Regular.Model;
using Regular.ViewModel;

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
                case RuleType.FreeText:
                    return SanitizeWord(regexRulePart.RawUserInputValue);
                case RuleType.SelectionSet:
                    // We'll need to break these up somehow
                    return "Test";
                case RuleType.AnyLetter:
                    if (regexRulePart.IsCaseSensitive) { }
                    // How do we handle case-sensitive?
                    return @"[a-zA-Z]";
                case RuleType.AnyDigit:
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
            if (String.IsNullOrEmpty(word)) return word;
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

        public static char[] Letters = new[] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l','m', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        public static int[] Numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public static string GenerateRandomExample(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Random random = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
            string randomExampleString = "Example: ";
            foreach (RegexRulePart regexRulePart in regexRuleParts)
            {
                switch (regexRulePart.RuleType)
                {
                    case RuleType.AnyLetter:
                        switch (regexRulePart.CaseSensitiveDisplayString)
                        {
                            case "UPPER CASE":
                                randomExampleString += Letters[random.Next(Letters.Length)].ToString().ToUpper();
                                break;
                            case "lower case":
                                randomExampleString += Letters[random.Next(Letters.Length)].ToString().ToLower();
                                break;
                            case "Any Case":
                                // Randomly pick any letter of random case
                                double randomDouble = random.NextDouble();
                                randomExampleString += randomDouble >= 0.5
                                    ? Letters[random.Next(Letters.Length)].ToString().ToLower()
                                    : Letters[random.Next(Letters.Length)].ToString().ToUpper();
                                break;
                            default:
                                break;
                        }
                        break;
                    case RuleType.AnyDigit:
                        randomExampleString += Numbers[random.Next(Numbers.Length)];
                        break;
                    case RuleType.FreeText:
                        randomExampleString += regexRulePart.RawUserInputValue;
                        break;
                    case RuleType.SelectionSet:
                        randomExampleString += regexRulePart.RawUserInputValue;
                        break;
                    default:
                        randomExampleString += "";
                        break;
                }
            }
            return randomExampleString;
        }
    }
}
