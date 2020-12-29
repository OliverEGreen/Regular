using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regular.Models
{
    public static class RegexRuleCache
    {
        public static Dictionary<string, ObservableCollection<RegexRule>> AllRegexRules { get; set; }
    }
}
