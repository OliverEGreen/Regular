using Regular.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regular.Model
{
    class RegexRules
    {
        public static Dictionary<string, ObservableCollection<RegexRule>> AllRegexRules { get; set; }
    }
}
