using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regular.Enums;
using Regular.Models;

namespace Regular.Services
{
    public static class DeserializationServices
    {
        private static RuleType GetRuleTypeFromString(string input)
        {
            switch (input)
            {
                case "AnyLetter":
                    return RuleType.AnyLetter;
                case "AnyDigit":
                    return RuleType.AnyDigit;
                case "AnyAlphanumeric":
                    return RuleType.AnyAlphanumeric;
                case "FreeText":
                    return RuleType.FreeText;
                case "SelectionSet":
                    return RuleType.SelectionSet;
                default:
                    return RuleType.AnyLetter;
            }
        }
        public static ObservableCollection<IRegexRulePart> DeserializeRegexRulePartsInExtensibleStorage(List<string> regexRulePartsString)
        {
            ObservableCollection<IRegexRulePart> regexRuleParts = new ObservableCollection<IRegexRulePart>();

            // Converting RuleParts from serialized strings to real RegexRuleParts
            foreach (string serializedString in regexRulePartsString)
            {
                List<string> serializedStringParts = serializedString.Split('`').ToList();
                RuleType regexRuleType = GetRuleTypeFromString(serializedStringParts[0]);

                regexRuleParts.Add(RegexRulePart.Create(regexRuleType));
            }
            return regexRuleParts;
        }
    }
}
