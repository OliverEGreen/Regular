using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regular.Enums;
using Regular.Model;
using Regular.ViewModel;

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
                regexRulePartList.Add($@"{regexRulePart.RuleType}`
                                            {regexRulePart.IsOptional}`
                                            {regexRulePart.IsCaseSensitive}`
                                            {regexRulePart.IsEditable}");
            }
            return regexRulePartList;
        }
        private static RuleType GetRuleTypeFromString(string input)
        {
            switch (input)
            {
                case "AnyLetter":
                    return RuleType.AnyLetter;
                case "AnyDigit":
                    return RuleType.AnyDigit;
                case "FreeText":
                    return RuleType.FreeText;
                case "SelectionSet":
                    return RuleType.SelectionSet;
                default:
                    return RuleType.AnyLetter;
            }
        }
        public static ObservableCollection<RegexRulePart> DeserializeRegexRulePartsInExtensibleStorage(List<string> regexRulePartsString)
        {
            ObservableCollection<RegexRulePart> regexRuleParts = new ObservableCollection<RegexRulePart>();

            // Converting RuleParts from serialized strings to real RegexRuleParts
            foreach (string serializedString in regexRulePartsString)
            {
                List<string> serializedStringParts = serializedString.Split('`').ToList();
                RuleType regexRuleType = GetRuleTypeFromString(serializedStringParts[0]);
                bool isOptional = Convert.ToBoolean(serializedStringParts[1]);
                bool isCaseSensitive = Convert.ToBoolean(serializedStringParts[2]);
                bool requiresUserInput = Convert.ToBoolean(serializedStringParts[3]);

                regexRuleParts.Add(new RegexRulePart(regexRuleType, isOptional, isCaseSensitive, requiresUserInput));
            }
            return regexRuleParts;
        }
    }
}
