using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regular.Models;

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
            return regexRuleParts.Select(regexRulePart => $@"{regexRulePart.RuleType}`
                                            {regexRulePart.IsOptional}`
                                            {regexRulePart.IsCaseSensitive}`
                                            {regexRulePart.IsEditable}")
                .ToList();
        }
    }
}
