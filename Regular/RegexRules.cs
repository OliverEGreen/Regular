using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    class RegexRules
    {
        public static Dictionary<string, ObservableCollection<RegexRule>> AllRegexRules { get; set; }
        public RegexRules()
        {

        }
    }
}
