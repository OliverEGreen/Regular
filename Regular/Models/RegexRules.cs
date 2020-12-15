using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regular.Models
{
    internal class RegexRules
    {
        public static Dictionary<string, ObservableCollection<RegexRule>> AllRegexRules { get; set; }
    }
}
