using System.Collections.Generic;
using System.Collections.ObjectModel;
using Regular.ViewModel;

namespace Regular.Model
{
    class RegexRules
    {
        public static Dictionary<string, ObservableCollection<RegexRule>> AllRegexRules { get; set; }
    }
}
