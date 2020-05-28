using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Services
{
    public static class DeserializationServices
    {
        public static ObservableCollection<RegexRulePart> DeserializeRegexRuleParts(List<string> regexRulePartsString)
        {
            ObservableCollection<RegexRulePart> regexRuleParts = new ObservableCollection<RegexRulePart>();

            // Converting RuleParts from serialized strings to real RegexRuleParts
            foreach (string serializedString in regexRulePartsString)
            {
                List<string> serializedStringParts = serializedString.Split('`').ToList();
                string rawUserInputValue = serializedStringParts[0];
                string regexRuleTypeString = serializedStringParts[1];
                bool isOptional = Convert.ToBoolean(serializedStringParts[2]);
                bool isCaseSensitive = Convert.ToBoolean(serializedStringParts[3]);
                RuleTypes ruleType = RuleTypes.None;
                switch (regexRuleTypeString)
                {
                    case "AnyLetter":
                        ruleType = RuleTypes.AnyLetter;
                        break;
                    case "SpecificLetter":
                        ruleType = RuleTypes.SpecificLetter;
                        break;
                    case "AnyNumber":
                        ruleType = RuleTypes.AnyNumber;
                        break;
                    case "SpecificNumber":
                        ruleType = RuleTypes.SpecificNumber;
                        break;
                    case "AnyCharacter":
                        ruleType = RuleTypes.AnyCharacter;
                        break;
                    case "SpecificCharacter":
                        ruleType = RuleTypes.SpecificCharacter;
                        break;
                    case "AnyFromSet":
                        ruleType = RuleTypes.AnyFromSet;
                        break;
                    case "Anything":
                        ruleType = RuleTypes.Anything;
                        break;
                    case "Dot":
                        ruleType = RuleTypes.Dot;
                        break;
                    case "Hyphen":
                        ruleType = RuleTypes.Hyphen;
                        break;
                    case "Underscore":
                        ruleType = RuleTypes.Underscore;
                        break;
                    default:
                        ruleType = RuleTypes.None;
                        break;
                }

                RegexRulePart regexRulePart = new RegexRulePart(rawUserInputValue, ruleType, isOptional, isCaseSensitive);
                regexRuleParts.Add(regexRulePart);
            }
            return regexRuleParts;
        }
    }
}
