using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Regular.Enums;
using Regular.Model;

namespace Regular.ViewModel
{
    public class RegexRule : INotifyPropertyChanged
    {
        private string name;
        private ObservableCollection<ObservableObject> targetCategoryIds;
        private ObservableCollection<RegexRulePart> regexRuleParts;
        private string trackingParameterName; // Eventually, this should be some kind of ID
        private string outputParameterName; // This should also be an ID
        private string toolTipString;
        private string regexString;
        private MatchType matchType;
        
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public string DateTimeCreated { get; private set; }
        public string CreatedBy { get; private set; }
        public string Guid { get; private set; }
        public ObservableCollection<ObservableObject> TargetCategoryIds
        {
            get => targetCategoryIds;
            set
            {
                targetCategoryIds = value;
                NotifyPropertyChanged("TargetCategoryIds");
            }
        }
        public string TrackingParameterName
        {
            get => trackingParameterName;
            set
            {
                trackingParameterName = value;
                NotifyPropertyChanged("TrackingParameterName");
            }
        }
        public string OutputParameterName
        {
            get => outputParameterName;
            set
            {
                outputParameterName = value;
                NotifyPropertyChanged("OutputParameterName");
            }
        }
        public string ToolTip
        {
            get
            {
                toolTipString = $"Rule Name: {name}" + Environment.NewLine +
                                $"Applies To: {String.Join(", ", targetCategoryIds)}" + Environment.NewLine +
                                $"Tracks Parameter : {trackingParameterName}" + Environment.NewLine +
                                $"Regex String: {RegexString}" + Environment.NewLine +
                                $"Writes To : {outputParameterName}" + Environment.NewLine +
                                $"Created By: {CreatedBy}" + Environment.NewLine +
                                $"Created At: {DateTimeCreated}";
                return toolTipString;
            }
            set
            {
                toolTipString = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("TargetCategoryIds");
                NotifyPropertyChanged("TrackingParameterName");
                NotifyPropertyChanged("OutputParameterName");
                NotifyPropertyChanged("ToolTip");
                NotifyPropertyChanged("RegexString");
            }
        }
        public string RegexString
        {
            get => regexString;
            set
            {
                regexString = value;
                NotifyPropertyChanged("RegexString");
            }
        }
        public ObservableCollection<RegexRulePart> RegexRuleParts
        {
            get => regexRuleParts;
            set
            {
                regexRuleParts = value;
                NotifyPropertyChanged("RegexRuleParts");
            }
        }
        public MatchType MatchType
        {
            get => matchType;
            set
            {
                matchType = value;
                NotifyPropertyChanged("MatchType");
            }
        }

        public RegexRule() { }

        public static RegexRule Create(string documentGuid, string guid = null)
        {
            return new RegexRule()
            {
                Guid = guid ?? new Guid().ToString(),
                Name = "",
                OutputParameterName = "",
                RegexRuleParts = new ObservableCollection<RegexRulePart>(),
                RegexString = "",
                TargetCategoryIds = ObservableObject.GetInitialCategories(documentGuid),
                ToolTip = "",
                TrackingParameterName = "",
                DateTimeCreated = DateTime.Now.ToString("r"),
                CreatedBy = Environment.UserName
            };
        }
        
        public static void Save(string documentGuid, RegexRule regexRule)
        {
            RegexRules.AllRegexRules[documentGuid].Add(regexRule);
        }
        public static RegexRule GetRuleById(string documentGuid, string regexRuleGuid)
        {
            ObservableCollection<RegexRule> documentRegexRules = GetDocumentRegexRules(documentGuid);
            return documentRegexRules?.FirstOrDefault(x => x.Guid == regexRuleGuid);
        }
        public static ObservableCollection<RegexRule> GetDocumentRegexRules(string documentGuid)
        {
            return RegexRules.AllRegexRules.ContainsKey(documentGuid) ? RegexRules.AllRegexRules[documentGuid] : null;
        }
        public static List<string> GetDocumentRegexRuleGuids(string documentGuid)
        {
            return GetDocumentRegexRules(documentGuid).Select(x => x.Guid).ToList();
        }
        public static void Update(string documentGuid, string regexRuleGuid, RegexRule newRegexRule)
        {
            // Takes a newly-generated RegexRule object and sets an existing rules values to match
            // To be used when updating an existing rule from the Rule Editor
            RegexRule existingRegexRule = GetRuleById(documentGuid, regexRuleGuid);
            if (existingRegexRule == null) return;

            existingRegexRule.OutputParameterName = newRegexRule.OutputParameterName;
            existingRegexRule.RegexRuleParts = newRegexRule.RegexRuleParts;
            existingRegexRule.RegexString = newRegexRule.RegexString;
            existingRegexRule.Name = newRegexRule.Name;
            existingRegexRule.TargetCategoryIds = newRegexRule.TargetCategoryIds;
            existingRegexRule.TrackingParameterName = newRegexRule.TrackingParameterName;
        }
        public static void Delete(string documentGuid, string regexRuleGuid)
        {
            // Deletes a RegexRule from the document's static cache
            if (!RegexRules.AllRegexRules.ContainsKey(documentGuid)) return;
            ObservableCollection<RegexRule> documentRegexRules = GetDocumentRegexRules(documentGuid);
            RegexRule regexRule = documentRegexRules.FirstOrDefault(x => x.Guid == regexRuleGuid);
            if (regexRule != null) documentRegexRules.Remove(regexRule);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
