using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Regular.Enums;
using Regular.Utilities;

namespace Regular.Models
{
    public class RegexRule : INotifyPropertyChanged
    {
        private string ruleName = "";
        private ObservableCollection<CategoryObject> targetCategoryObjects = new ObservableCollection<CategoryObject>();
        private ObservableCollection<IRegexRulePart> regexRuleParts = new ObservableCollection<IRegexRulePart>();
        private ParameterObject trackingParameterObject = new ParameterObject {ParameterObjectId = -1, ParameterObjectName = ""};
        private string toolTipString = "";
        private string regexString = "";
        private MatchType matchType = MatchType.ExactMatch;
        private bool isFrozen = false;

        public string RuleName
        {
            get => ruleName;
            set
            {
                ruleName = value;
                NotifyPropertyChanged("RuleName");
            }
        }
        public ObservableCollection<CategoryObject> TargetCategoryObjects
        {
            get => targetCategoryObjects;
            set
            {
                targetCategoryObjects = value;
                NotifyPropertyChanged("TargetCategoryObjects");
            }
        }
        public ObservableCollection<IRegexRulePart> RegexRuleParts
        {
            get => regexRuleParts;
            set
            {
                regexRuleParts = value;
                NotifyPropertyChanged("RegexRuleParts");
            }
        }
        public ParameterObject TrackingParameterObject
        {
            get => trackingParameterObject;
            set
            {
                if (value != null)
                {
                    trackingParameterObject = value;
                    NotifyPropertyChanged("TrackingParameterObject");
                }
            }
        }
        
        public string ToolTip
        {
            get
            {
                toolTipString = $"Rule Name: {RuleName}" + Environment.NewLine +
                                $"Applies To: {string.Join(", ", TargetCategoryObjects.Where(x => x.IsChecked).Select(x => x.CategoryObjectName))}" + Environment.NewLine +
                                $"Tracks Parameter : {TrackingParameterObject.ParameterObjectName}" + Environment.NewLine +
                                $"Regular Expression: {RegexString}" + Environment.NewLine +
                                $"Created By: {CreatedBy}" + Environment.NewLine +
                                $"Created At: {DateTimeCreated}" + Environment.NewLine +
                                $"Last Modified: {LastModified}";
                return toolTipString;
            }
            set
            {
                toolTipString = value;
                NotifyPropertyChanged("ToolTip");
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

        public bool IsStagingRule { get; set; }
        public string DateTimeCreated { get; set; } = DateTime.Now.ToString("r");
        public string LastModified { get; set; } = DateTime.Now.ToString("r");
        public string CreatedBy { get; set; } = Environment.UserName;
        public string RuleGuid { get; private set; } = Guid.NewGuid().ToString();
        
        public static RegexRule Create(string documentGuid)
        {
            RegexRule regexRule = new RegexRule
            {
                TargetCategoryObjects = CategoryUtils.GetInitialCategories(documentGuid)
            };
            return regexRule;
        }

        public static void Save(string documentGuid, RegexRule regexRule)
        {
            RegularApp.RegexRuleCacheService.AddRule(documentGuid, regexRule);
            ExtensibleStorageUtils.SaveRegexRuleToExtensibleStorage(documentGuid, regexRule);
        }

        public static void Delete(string documentGuid, RegexRule regexRule)
        {
            RegularApp.RegexRuleCacheService.RemoveRule(documentGuid, regexRule.RuleGuid);
            ExtensibleStorageUtils.DeleteRegexRuleFromExtensibleStorage(documentGuid, regexRule.RuleGuid);
        }

        public static RegexRule Update(string documentGuid, RegexRule existingRegexRule, RegexRule stagingRegexRule)
        {
            // We copy all properties over from the staging rule to the existing rule
            existingRegexRule = DeepCopyRegexRule(stagingRegexRule);
            
            RegularApp.RegexRuleCacheService.UpdateRule(documentGuid, existingRegexRule);
            ExtensibleStorageUtils.UpdateRegexRuleInExtensibleStorage(documentGuid, existingRegexRule.RuleGuid, stagingRegexRule);

            existingRegexRule.LastModified = DateTime.Now.ToString("r");
            return existingRegexRule;
        }

        public static RegexRule DeepCopyRegexRule(RegexRule ruleToCopy)
        {
            // Deep copy has issues because it instantites new RegularUpdater objects
            // whereas we just want a reference to the original object when we update it
            string ruleGuid = ruleToCopy.RuleGuid;
            ruleToCopy = SerializationUtils.DeepCopyObject(ruleToCopy);
            ruleToCopy.RuleGuid = ruleGuid;
            return ruleToCopy;
        }
        
        public static RegexRule Duplicate(string documentGuid, RegexRule sourceRegexRule, bool isStagingRule)
        {
            // Helper method to ensure duplicate rules always have a unique name
            string GenerateRegexRuleDuplicateName()
            {
                List<string> documentRegexRuleNames = RegularApp.RegexRuleCacheService
                    .GetDocumentRules(documentGuid)
                    .Select(x => x.RuleName)
                    .ToList();
                string copyName = $"{sourceRegexRule.RuleName} Copy";
                return documentRegexRuleNames.Contains(copyName) ? $"{copyName} Copy" : copyName;
            }
            
            // Returns a deep copy of an existing RegexRule, but with a new GUID
            RegexRule duplicateRegexRule = DeepCopyRegexRule(sourceRegexRule);
            duplicateRegexRule.IsStagingRule = isStagingRule;
            
            if (!isStagingRule)
            {
                duplicateRegexRule.RuleGuid = Guid.NewGuid().ToString();
                duplicateRegexRule.RuleName = GenerateRegexRuleDuplicateName();
            }
            
            return duplicateRegexRule;
        }
       
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
