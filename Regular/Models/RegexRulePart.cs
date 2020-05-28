using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Models
{
    public class RegexRulePart
    {
        public string RawUserInputValue { get; set; }
        public RuleTypes RuleType { get; set; }
        public bool IsOptional { get; set; }
        public string DisplayString { get; set; }
        public bool IsCaseSensitive { get; set; }

        // Our constructor with all the information we want when creating a RegexRulePart
        public RegexRulePart(string rawUserInputValue, RuleTypes ruleType, bool isOptional, bool isCaseSensitive)
        {
            RawUserInputValue = rawUserInputValue;
            RuleType = ruleType;
            IsOptional = IsOptional;
            DisplayString = ruleType.ToString();
            IsCaseSensitive = isCaseSensitive;
        }
    }
}
