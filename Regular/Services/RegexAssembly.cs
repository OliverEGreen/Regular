using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.UI;
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
            string regexPartOutput = "";
            string optionalModifier = regexRulePart.IsOptional ? "?" : "";
            string caseSensitiveModifier = regexRulePart.IsCaseSensitive ? "(?i)" : "";
            switch (regexRulePart.RuleType)
            {
                case RuleType.FreeText:
                    regexPartOutput += SanitizeWord(regexRulePart.DisplayText) + caseSensitiveModifier;
                    break;
                case RuleType.SelectionSet:
                    regexPartOutput += "Test";
                    break;
                case RuleType.AnyLetter:
                    switch (regexRulePart.CaseSensitiveDisplayString)
                    {
                        case "UPPER CASE":
                            regexPartOutput += "[A-Z]";
                            break;
                        case "lower case":
                            regexPartOutput += "[a-z]";
                            break;
                        case "Any Case":
                            regexPartOutput += "[A-Za-z]";
                            break;
                        default:
                            break;
                    }
                    break;
                case RuleType.AnyDigit:
                    regexPartOutput += @"\d";
                    break;
                default:
                    break;
            }
            return regexPartOutput + optionalModifier;
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
        public static string AssembleRegexString(RegexRule regexRule)
        {
            string regexString = "";
            foreach(RegexRulePart regexRulePart in regexRule.RegexRuleParts) { regexString += GetRegexPartFromRuleType(regexRulePart); }

            string start = ".*";
            string end = ".*";

            switch (regexRule.MatchType)
            {
                case MatchType.ExactMatch:
                    start = "^";
                    end = "$";
                    break;
                case MatchType.PartialMatch:
                    break;
                case MatchType.MatchAtBeginning:
                    start = "^";
                    break;
                case MatchType.MatchAtEnd:
                    end = "$";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // We pad out the final sting to facilitate the MatchType specified by the user.
            TaskDialog.Show("Test", $"{start + regexString + end}");
            return start + regexString + end;
        }

        public static char[] Letters = new[] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l','m', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        public static int[] Numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public static string GenerateRandomExample(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Random random = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
            string randomExampleString = "Example: ";
            foreach (RegexRulePart regexRulePart in regexRuleParts)
            {
                double randomDouble = random.NextDouble();
                if (regexRulePart.IsOptional && randomDouble > 0.5) continue;
                switch (regexRulePart.RuleType)
                {
                    case RuleType.AnyLetter:
                        string randomLetter = Letters[random.Next(Letters.Length)].ToString();
                        switch (regexRulePart.CaseSensitiveDisplayString)
                        {
                            case "UPPER CASE":
                                randomExampleString += randomLetter.ToUpper();
                                break;
                            case "lower case":
                                randomExampleString += randomLetter.ToLower();
                                break;
                            case "Any Case":
                                // Randomly pick any letter of random case
                                double anyCaseRandom = random.NextDouble();
                                randomExampleString += anyCaseRandom >= 0.5
                                    ? randomLetter.ToLower()
                                    : randomLetter.ToUpper();
                                break;
                            default:
                                break;
                        }
                        break;
                    case RuleType.AnyDigit:
                        randomExampleString += Numbers[random.Next(Numbers.Length)];
                        break;
                    case RuleType.FreeText:
                        if (regexRulePart.DisplayText == "Free Text") continue;
                        randomExampleString += regexRulePart.DisplayText;
                        break;
                    case RuleType.SelectionSet:
                        randomExampleString += regexRulePart.DisplayText;
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
