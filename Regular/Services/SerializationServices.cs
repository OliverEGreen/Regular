using Regular.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Regular.Services
{
    public static class SerializationServices
    {
        public static IList<string> ConvertListToIList(List<string> inputList)
        {
            IList<string> iList = new List<string>();
            foreach(string inputString in inputList) { iList.Add(inputString); }
            return iList;
        }
        public static IList<string> SerializeRegexRuleParts(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            IList<string> regexRulePartList = new List<string>();
            foreach (RegexRulePart regexRulePart in regexRuleParts)
            {
                regexRulePartList.Add($@"{regexRulePart.RuleType}`{regexRulePart.RawUserInputValue}`{regexRulePart.IsOptional}`{regexRulePart.IsCaseSensitive}`{regexRulePart.IsEditable}");
            }
            return regexRulePartList;
        }
        private static RuleTypes GetRuleTypeFromString(string input)
        {
            switch (input)
            {
                case "AnyLetter":
                    return RuleTypes.AnyLetter;
                case "AnyDigit":
                    return RuleTypes.AnyDigit;
                case "FreeText":
                    return RuleTypes.FreeText;
                case "SelectionSet":
                    return RuleTypes.SelectionSet;
                default:
                    return RuleTypes.AnyLetter;
            }
        }
        public static ObservableCollection<RegexRulePart> DeserializeRegexRulePartsInExtensibleStorage(List<string> regexRulePartsString)
        {
            ObservableCollection<RegexRulePart> regexRuleParts = new ObservableCollection<RegexRulePart>();

            // Converting RuleParts from serialized strings to real RegexRuleParts
            foreach (string serializedString in regexRulePartsString)
            {
                List<string> serializedStringParts = serializedString.Split('`').ToList();
                RuleTypes regexRuleType = GetRuleTypeFromString(serializedStringParts[0]);
                string rawUserInputValue = serializedStringParts[1];
                bool isOptional = Convert.ToBoolean(serializedStringParts[2]);
                bool isCaseSensitive = Convert.ToBoolean(serializedStringParts[3]);
                bool requiresUserInput = Convert.ToBoolean(serializedStringParts[4]);

                regexRuleParts.Add(new RegexRulePart(regexRuleType, isOptional, isCaseSensitive, requiresUserInput, rawUserInputValue));
            }
            return regexRuleParts;
        }
    }
}
