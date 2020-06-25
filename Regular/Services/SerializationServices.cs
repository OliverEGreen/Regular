using System.Collections.Generic;
using System.Collections.ObjectModel;
using Regular.ViewModel;

namespace Regular.Services
{
    public static class SerializationServices
    {
        public static IList<T> ConvertListToIList<T>(List<T> inputList)
        {
            IList<T> iList = new List<T>();
            foreach(T input in inputList) { iList.Add(input); }
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
