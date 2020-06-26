using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Regular.Enums;
using Regular.Model;
using Regular.Services;

namespace Regular.ViewModel
{
    public class RegexRule : INotifyPropertyChanged
    {
        private string ruleName;
        private ObservableCollection<CategoryObject> targetCategoryIds;
        private ObservableCollection<RegexRulePart> regexRuleParts;
        private ParameterObject trackingParameterObject;
        private ParameterObject outputParameterObject;
        private string toolTipString;
        private string regexString;
        private MatchType matchType;
        private bool isFrozen;
        
        public string RuleName
        {
            get => ruleName;
            set
            {
                ruleName = value;
                NotifyPropertyChanged("RuleName");
            }
        }
        public string DateTimeCreated { get; private set; }
        public string CreatedBy { get; private set; }
        public string RuleGuid { get; private set; }
        public ObservableCollection<CategoryObject> TargetCategoryIds
        {
            get => targetCategoryIds;
            set
            {
                targetCategoryIds = value;
                NotifyPropertyChanged("TargetCategoryIds");
            }
        }

        public ParameterObject TrackingParameterObject
        {
            get => trackingParameterObject;
            set
            {
                trackingParameterObject = value;
                NotifyPropertyChanged("TrackingParameterObject");
            }
        }
        public ParameterObject OutputParameterObject
        {
            get => outputParameterObject;
            set
            {
                outputParameterObject = value;
                NotifyPropertyChanged("OutputParameterObject");
            }
        }
        public string ToolTip
        {
            get
            {
                toolTipString = $"Rule RuleName: {RuleName}" + Environment.NewLine +
                                $"Applies To: {String.Join(", ", TargetCategoryIds.Select(x => x.CategoryObjectId))}" + Environment.NewLine +
                                $"Tracks Parameter : {TrackingParameterObject}" + Environment.NewLine +
                                $"Regex String: {RegexString}" + Environment.NewLine +
                                $"Writes To : {OutputParameterObject}" + Environment.NewLine +
                                $"Created By: {CreatedBy}" + Environment.NewLine +
                                $"Created At: {DateTimeCreated}";
                return toolTipString;
            }
            set
            {
                toolTipString = value;
                NotifyPropertyChanged("RuleName");
                NotifyPropertyChanged("TargetCategoryIds");
                NotifyPropertyChanged("TrackingParameterObject");
                NotifyPropertyChanged("OutputParameterObject");
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
        public bool IsFrozen
        {
            get => isFrozen;
            set
            {
                isFrozen = value;
                NotifyPropertyChanged("IsFrozen");
            }
        }
        

        public static RegexRule Create(string documentGuid, string guid = null)
        {
            return new RegexRule()
            {
                // If given a GUID, a rule is being recreated, otherwise a new rule is being created
                RuleGuid = guid ?? Guid.NewGuid().ToString(),
                RuleName = "",
                TargetCategoryIds = CategoryObject.GetInitialCategories(documentGuid),
                TrackingParameterObject = new ParameterObject { ParameterObjectId = -1, ParameterObjectName = "" },
                OutputParameterObject = new ParameterObject { ParameterObjectId = -1, ParameterObjectName = "" },
                MatchType = MatchType.ExactMatch,
                RegexRuleParts = new ObservableCollection<RegexRulePart>(),
                RegexString = "",
                IsFrozen = false,

                ToolTip = "",
                DateTimeCreated = DateTime.Now.ToString("r"),
                CreatedBy = Environment.UserName
            };
        }
        public static void Save(string documentGuid, RegexRule regexRule)
        {
            RegexRules.AllRegexRules[documentGuid].Add(regexRule);
            ExtensibleStorageServices.SaveRegexRuleToExtensibleStorage(documentGuid, regexRule);
            DMTriggerServices.AddTrigger(documentGuid, regexRule);
            
            // TODO: Check this rule is created as we want
            ParameterServices.CreateProjectParameter(documentGuid, regexRule.OutputParameterObject.ParameterObjectName, regexRule.TargetCategoryIds);
        }
        public static RegexRule Duplicate(string documentGuid, RegexRule sourceRegexRule)
        {
            // Helper method to ensure duplicate rules always have a unique name
            string GenerateRegexRuleDuplicateName(string regexRuleName)
            {
                List<string> documentRegexRuleNames = GetDocumentRegexRules(documentGuid).Select(x => x.RuleName).ToList();
                string copyName = $"{sourceRegexRule.RuleName} Copy";
                return documentRegexRuleNames.Contains(copyName) ? $"{copyName} Copy" : copyName;
            }
            
            // Returns a copy of an existing RegexRule, but with a new GUID
            RegexRule duplicateRegexRule = Create(documentGuid);
            duplicateRegexRule.RuleName = GenerateRegexRuleDuplicateName(sourceRegexRule.RuleName);
            duplicateRegexRule.TargetCategoryIds = sourceRegexRule.TargetCategoryIds;
            duplicateRegexRule.TrackingParameterObject = sourceRegexRule.TrackingParameterObject;
            duplicateRegexRule.OutputParameterObject = sourceRegexRule.OutputParameterObject;
            duplicateRegexRule.MatchType = sourceRegexRule.MatchType;
            duplicateRegexRule.RegexRuleParts = sourceRegexRule.RegexRuleParts;
            duplicateRegexRule.RegexString = sourceRegexRule.RegexString;
            duplicateRegexRule.IsFrozen = sourceRegexRule.IsFrozen;
            
            return duplicateRegexRule;
        }
        public static RegexRule GetRuleById(string documentGuid, string regexRuleGuid)
        {
            ObservableCollection<RegexRule> documentRegexRules = GetDocumentRegexRules(documentGuid);
            return documentRegexRules?.FirstOrDefault(x => x.RuleGuid == regexRuleGuid);
        }
        public static ObservableCollection<RegexRule> GetDocumentRegexRules(string documentGuid)
        {
            return RegexRules.AllRegexRules.ContainsKey(documentGuid) ? RegexRules.AllRegexRules[documentGuid] : null;
        }
        public static List<string> GetDocumentRegexRuleGuids(string documentGuid)
        {
            return GetDocumentRegexRules(documentGuid).Select(x => x.RuleGuid).ToList();
        }
        public static void Update(string documentGuid, string regexRuleGuid, RegexRule newRegexRule)
        {
            // Takes a newly-generated RegexRule object and sets an existing rules values to match
            // To be used when updating an existing rule from the Rule Editor
            RegexRule existingRegexRule = GetRuleById(documentGuid, regexRuleGuid);
            if (existingRegexRule == null) return;

            existingRegexRule.RuleName = newRegexRule.RuleName;
            existingRegexRule.TargetCategoryIds = newRegexRule.TargetCategoryIds;
            existingRegexRule.TrackingParameterObject = newRegexRule.TrackingParameterObject;
            existingRegexRule.OutputParameterObject = newRegexRule.OutputParameterObject;
            existingRegexRule.MatchType = newRegexRule.MatchType;
            existingRegexRule.RegexRuleParts = newRegexRule.RegexRuleParts;
            existingRegexRule.RegexString = newRegexRule.RegexString;
            existingRegexRule.IsFrozen = newRegexRule.IsFrozen;

            ExtensibleStorageServices.UpdateRegexRuleInExtensibleStorage(documentGuid, existingRegexRule.RuleGuid, newRegexRule);
            DMTriggerServices.UpdateAllTriggers(documentGuid);
        }
        public static void Delete(string documentGuid, string regexRuleGuid)
        {
            // Deletes a RegexRule from the document's static cache
            if (!RegexRules.AllRegexRules.ContainsKey(documentGuid)) return;
            ObservableCollection<RegexRule> documentRegexRules = GetDocumentRegexRules(documentGuid);
            RegexRule regexRule = documentRegexRules.FirstOrDefault(x => x.RuleGuid == regexRuleGuid);
            if (regexRule != null) documentRegexRules.Remove(regexRule);

            DMTriggerServices.DeleteTrigger(documentGuid, regexRule);
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
