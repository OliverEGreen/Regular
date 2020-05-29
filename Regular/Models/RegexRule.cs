using System.Collections.ObjectModel;

namespace Regular.Models
{
    public class RegexRule
    {
        public string Guid { get; }
        public string RuleName { get; set; }
        public string TargetCategoryName { get; set; }
        public string TrackingParameterName { get; set; }
        public string OutputParameterName { get; set; }
        public string RegexString { get; set; }
        public ObservableCollection<RegexRulePart> RegexRuleParts { get; set; }

        // Constructor, when user creates a new rule we require (and set) the following information
        public RegexRule(string ruleName, string targetCategoryName, string trackingParameterName, string outputParameterName, string regexString, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Guid = System.Guid.NewGuid().ToString();
            RuleName = ruleName;
            TargetCategoryName = targetCategoryName;
            TrackingParameterName = trackingParameterName;
            OutputParameterName = outputParameterName;
            RegexString = regexString;
            RegexRuleParts = regexRuleParts;
        }
        public RegexRule(string guid, string ruleName, string targetCategoryName, string trackingParameterName, string outputParameterName, string regexString, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Guid = guid;
            RuleName = ruleName;
            TargetCategoryName = targetCategoryName;
            TrackingParameterName = trackingParameterName;
            OutputParameterName = outputParameterName;
            RegexString = regexString;
            RegexRuleParts = regexRuleParts;
        }
    }
}
