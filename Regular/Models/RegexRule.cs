using System;
using System.Collections.ObjectModel;

namespace Regular.Models
{
    public class RegexRule
    {
        public string Guid { get; }
        public string Name { get; set; }
        public string TargetCategoryNames { get; set; }
        public string TrackingParameterName { get; set; }
        public string OutputParameterName { get; set; }
        public string ToolTipString { get; }
        public string RegexString { get; set; }
        public ObservableCollection<RegexRulePart> RegexRuleParts { get; set; }
        
        // Constructor, when user creates a new rule we require (and set) the following information
        public RegexRule(string ruleName, string targetCategoryName, string trackingParameterName, string outputParameterName, string regexString, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Guid = System.Guid.NewGuid().ToString();
            Name = ruleName;
            TargetCategoryNames = targetCategoryName;
            TrackingParameterName = trackingParameterName;
            OutputParameterName = outputParameterName;
            RegexString = regexString;
            RegexRuleParts = regexRuleParts;
            ToolTipString = $"Rule Name: {Name}" + Environment.NewLine +
                            $"Applies To: {String.Join(", ", TargetCategoryNames)}" + Environment.NewLine +
                            $"Created By: {Environment.UserName}" + Environment.NewLine +
                            $"Created At: {DateTime.Now.ToString("r")}" + Environment.NewLine +
                            $"Regex String: {RegexString}";
        }
        // Constructor when recreating an existing rule from storage
        public RegexRule(string guid, string ruleName, string targetCategoryName, string trackingParameterName, string outputParameterName, string regexString, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Guid = guid;
            Name = ruleName;
            TargetCategoryNames = targetCategoryName;
            TrackingParameterName = trackingParameterName;
            OutputParameterName = outputParameterName;
            RegexString = regexString;
            RegexRuleParts = regexRuleParts;
        }
    }
}
