using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public static class RegexRuleManager
    {
        //A central class with CRUD functionality to manage the document's RegexRules
        public static RegexRule CreateRegexRule()
        {
            return null;
        }
        public static RegexRule FetchRegexRule(string guid)
        {
            return null;
        }
        public static ObservableCollection<RegexRule> FetchAllRegexRules(string guid)
        {
            return RegularApp.AllRegexRules[guid];
        }
        public static void UpdateRegexRule(string guid)
        {
            return;
        }
        public static void DeleteRegexRule(string guid)
        {

        }
    }
}
