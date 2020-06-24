using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }
}
