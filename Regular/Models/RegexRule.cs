using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Regular.Models
{
    class RegexRule
    {
        //Constructor, when user creates a new rule we require the following information
        public RegexRule(string ruleName, Category targetCategory, Parameter trackingParameter, string outputParameterName) { }
        public RegexRule(string ruleName, Category targetCategory, Parameter trackingParameter, Parameter outputParameter) { }
        public string regexString = "";
        public List<RegexRulePart> regexRuleParts = new List<RegexRulePart>();
        public bool isCaseSensitive = false;
    }
}
