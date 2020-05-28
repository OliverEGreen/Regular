using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Services
{
    public static class SerializationServices
    {
        public static IList<string> SerializeRegexRuleParts(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            IList<string> regexRulePartList = new List<string>();
            foreach (RegexRulePart regexRulePart in regexRuleParts)
            {
                regexRulePartList.Add($@"{regexRulePart.RawUserInputValue}`{regexRulePart.RuleType}`{regexRulePart.IsOptional}`{regexRulePart.IsCaseSensitive}");
            }
            return regexRulePartList;
        }
    }
}
