using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regular.Enums;
using Regular.Models;
using static Regular.Enums.RuleType;

namespace Regular.Utilities
{
    public static class RegexAssemblyUtils
    {
        private static readonly List<string> SpecialCharacters = new List<string> { @".", @"\", @"*", @"+", @"?", @"|", @"(", @")", @"[", @"]", @"^", @"{", @"}" };
        private static string GetRegexPartFromRuleType(IRegexRulePart regexRulePart)
        {
            string regexPartOutput = "";
            string optionalModifierStart = regexRulePart.IsOptional ? @"(" : "";
            string optionalModifierEnd = regexRulePart.IsOptional ? @")?" : "";
            string caseSensitiveModifierStart = regexRulePart.IsCaseSensitive ? @"(?-i)" : @"(?i)";
            string caseSensitiveModifierEnd = regexRulePart.IsCaseSensitive ? @"(?i)" : @"(?-i)";
            switch (regexRulePart.RuleType)
            {
                case AnyLetter:
                    switch (regexRulePart.CaseSensitivityMode)
                    {
                        case CaseSensitivity.UpperCase:
                            regexPartOutput += @"[A-Z]";
                            break;
                        case CaseSensitivity.AnyCase:
                            regexPartOutput += @"[A-Za-z]";
                            break;
                        case CaseSensitivity.LowerCase:
                            regexPartOutput += @"[a-z]";
                            break;
                        case CaseSensitivity.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case AnyAlphanumeric:
                    switch (regexRulePart.CaseSensitivityMode)
                    {
                        case CaseSensitivity.UpperCase:
                            regexPartOutput += "[A-Z0-9]";
                            break;
                        case CaseSensitivity.AnyCase:
                            regexPartOutput += "[A-Za-z0-9]";
                            break;
                        case CaseSensitivity.LowerCase:
                            regexPartOutput += "[a-z0-9]";
                            break;
                        case CaseSensitivity.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case AnyDigit:
                    regexPartOutput += @"\d";
                    break;
                case CustomText:
                    regexPartOutput += caseSensitiveModifierStart +
                                       SanitizeWord(regexRulePart.RawUserInputValue) +
                                       caseSensitiveModifierEnd;
                                       
                    break;
                case OptionSet:
                    List<string> options = regexRulePart.Options
                        .Select(x => SanitizeWord(x.OptionValue))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();
                    if (options.Count < 1) break;
                    regexPartOutput = caseSensitiveModifierStart +
                                      $"({string.Join(@"|", options)})" +
                                      caseSensitiveModifierEnd;
                    break;
                case FullStop:
                    regexPartOutput = SanitizeCharacter(@".");
                    break;
                case Hyphen:
                    regexPartOutput = SanitizeCharacter(@"-");
                    break;
                case Underscore:
                    regexPartOutput = SanitizeCharacter(@"_");
                    break;
                case OpenParenthesis:
                    regexPartOutput = SanitizeCharacter(@"(");
                    break;
                case CloseParenthesis:
                    regexPartOutput = SanitizeCharacter(@")");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return optionalModifierStart + regexPartOutput + optionalModifierEnd;
        }
        private static string SanitizeCharacter(string character)
        {
            return SpecialCharacters.Contains(character) ? $@"\{character}" : character;
        }
        private static string SanitizeWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return word;
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
            foreach(IRegexRulePart regexRulePart in regexRule.RegexRuleParts) { regexString += GetRegexPartFromRuleType(regexRulePart); }

            string start = "^.*";
            string end = ".*$";

            switch (regexRule.MatchType)
            {
                case MatchType.ExactMatch:
                    start = "^";
                    end = "$";
                    break;
                case MatchType.MatchAnywhere:
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
            return start + regexString + end;
        }

        public static char[] Letters = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l','m', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        public static int[] Numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static char[] Characters = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','0','1','2','3','4','5','6','7','8','9'};
        
        public static string GenerateRandomExample(ObservableCollection<IRegexRulePart> regexRuleParts)
        {
            Random random = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
            string randomExampleString = "";
            foreach (IRegexRulePart regexRulePart in regexRuleParts)
            {
                double randomDouble = random.NextDouble();
                double anyCaseRandom = random.NextDouble();
                if (regexRulePart.IsOptional && randomDouble > 0.5) continue;
                switch (regexRulePart.RuleType)
                {
                    case AnyLetter:
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
                                randomExampleString += anyCaseRandom >= 0.5
                                    ? randomLetter.ToLower()
                                    : randomLetter.ToUpper();
                                break;
                        }
                        break;
                    case AnyDigit:
                        randomExampleString += Numbers[random.Next(Numbers.Length)];
                        break;
                    case AnyAlphanumeric:
                        string randomCharacter = Characters[random.Next(Characters.Length)].ToString();
                        switch (regexRulePart.CaseSensitiveDisplayString)
                        {
                            case "UPPER CASE":
                                randomExampleString += randomCharacter.ToUpper();
                                break;
                            case "lower case":
                                randomExampleString += randomCharacter.ToLower();
                                break;
                            case "Any Case":
                                // Randomly pick any letter of random case
                                randomExampleString += anyCaseRandom >= 0.5
                                    ? randomCharacter.ToLower()
                                    : randomCharacter.ToUpper();
                                break;
                        }
                        break;
                    case CustomText:
                        if (regexRulePart.IsCaseSensitive)
                        {
                            randomExampleString += regexRulePart.RawUserInputValue;
                            break;
                        }
                        randomExampleString += anyCaseRandom >= 0.5
                            ? regexRulePart.RawUserInputValue.ToLower()
                            : regexRulePart.RawUserInputValue.ToUpper();
                        break;
                    case OptionSet:
                        List<string> values = regexRulePart.Options
                            .Select(x => x.OptionValue)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToList();
                        if (values.Count < 1) break;
                        string randomValue = values.OrderBy(s => Guid.NewGuid()).First();
                        if (regexRulePart.IsCaseSensitive)
                        {
                            randomExampleString += randomValue;
                        }
                        else
                        {
                            randomExampleString += anyCaseRandom >= 0.5
                                ? randomValue.ToLower()
                                : randomValue.ToUpper();
                        }
                        break;
                    case FullStop:
                        randomExampleString += @".";
                        break;
                    case Hyphen:
                        randomExampleString += @"-";
                        break;
                    case Underscore:
                        randomExampleString += @"_";
                        break;
                    case OpenParenthesis:
                        randomExampleString += @"(";
                        break;
                    case CloseParenthesis:
                        randomExampleString += @")";
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
