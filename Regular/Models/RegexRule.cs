using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Regular.Models
{
    public class RegexRule : INotifyPropertyChanged
    {
        private string name;
        private List<string> targetCategoryNames;
        private ObservableCollection<RegexRulePart> regexRuleParts;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public string Guid { get; set; }
        public List<string> TargetCategoryNames
        {
            get { return targetCategoryNames; }
            set
            {
                targetCategoryNames = value;
                NotifyPropertyChanged("TargetCategoryNames");
            }
        }
        public string TrackingParameterName { get; set; }
        public string OutputParameterName { get; set; }
        public string ToolTipString { get; set; }
        public string RegexString { get; set; }
        public ObservableCollection<RegexRulePart> RegexRuleParts
        {
            get { return regexRuleParts; }
            set
            {
                regexRuleParts = value;
                NotifyPropertyChanged("RegexRuleParts");
            }
        }

        // Constructor, when user creates a new rule we require (and set) the following information
        public RegexRule(string ruleName, List<string> targetCategoryNames, string trackingParameterName, string outputParameterName, string regexString, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Guid = System.Guid.NewGuid().ToString();
            Name = ruleName;
            TargetCategoryNames = targetCategoryNames;
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
        public RegexRule(string guid, string ruleName, List<string> targetCategoryNames, string trackingParameterName, string outputParameterName, string regexString, ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Guid = guid;
            Name = ruleName;
            //TargetCategoryNames = targetCategoryName;
            TrackingParameterName = trackingParameterName;
            OutputParameterName = outputParameterName;
            RegexString = regexString;
            RegexRuleParts = regexRuleParts;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
