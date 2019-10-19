using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Models
{
    class RegexRulePart
    {
        public RegexRulePart(string rulePartName, RuleTypes ruleType) { }
        //Lets the user decide whether a rule part can be applied optionally
        public bool isOptional = false;
    }
}
