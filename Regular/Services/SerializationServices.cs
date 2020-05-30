using Regular.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regular.Services
{
    public static class SerializationServices
    {
        public static IList<string> SerializeRegexRuleParts(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            IList<string> regexRulePartList = new List<string>();
            foreach (RegexRulePart regexRulePart in regexRuleParts)
            {
                regexRulePartList.Add($@"{regexRulePart.RuleType}`{regexRulePart.RawUserInputValue}`{regexRulePart.IsOptional}`{regexRulePart.IsCaseSensitive}`{regexRulePart.RequiresUserInput}");
            }
            return regexRulePartList;
        }
    }
}
