using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Regular.Models
{
    public class RegexRule
    {
        public string Guid { get; }
        public string RuleName { get; set; }
        public Category TargetCategory { get; set; }
        public string TrackingParameterName { get; set; }
        public string OutputParameterName { get; set; }
        public string RegexString { get; set; }
        public ObservableCollection<RegexRulePart> RegexRuleParts { get; set; }
        public bool IsCaseSensitive { get; set; }

        // Constructor, when user creates a new rule we require (and set) the following information
        public RegexRule(string ruleName, Category targetCategory, string trackingParameterName, string outputParameterName)
        {
            Guid = System.Guid.NewGuid().ToString();
            RuleName = ruleName;
            TargetCategory = targetCategory;
            TrackingParameterName = trackingParameterName;
            OutputParameterName = outputParameterName;
        }
    }
}
