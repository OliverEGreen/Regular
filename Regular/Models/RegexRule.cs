using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Regular.Models
{
    public class RegexRule : INotifyPropertyChanged
    {
        private string name;
        private string guid;
        private List<string> targetCategoryNames;
        private string trackingParameterName;
        private string outputParameterName;
        private string toolTipString;
        private string regexString;
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
        public string Guid
        {
            get { return guid; }
            set
            {
                guid = value;
                NotifyPropertyChanged("Guid");
            }
        }
        public List<string> TargetCategoryNames
        {
            get { return targetCategoryNames; }
            set
            {
                targetCategoryNames = value;
                NotifyPropertyChanged("TargetCategoryName");
            }
        }
        public string TrackingParameterName
        {
            get { return trackingParameterName; }
            set
            {
                trackingParameterName = value;
                NotifyPropertyChanged("TrackingParameterName");
            }
        }
        public string OutputParameterName
        {
            get { return outputParameterName; }
            set
            {
                outputParameterName = value;
                NotifyPropertyChanged("OutputParameterName");
            }
        }
        public string ToolTipString
        {
            get { return toolTipString; }
            set
            {
                toolTipString = value;
                NotifyPropertyChanged("ToolTipString");
            }
        }
        public string RegexString
        {
            get { return regexString; }
            set
            {
                regexString = value;
                NotifyPropertyChanged("RegexString");
            }
        }
        public ObservableCollection<RegexRulePart> RegexRuleParts
        {
            get { return regexRuleParts; }
            set { regexRuleParts = value; }
        }

        public RegexRule()
        {

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
