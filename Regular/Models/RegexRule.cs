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
        private ParameterObject outputParameterObject = new ParameterObject { ParameterObjectId = -1, ParameterObjectName = "" };
        private string toolTipString = "";
        private string regexString = "";
        private MatchType matchType = MatchType.ExactMatch;
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
                else
                {
                    int x = 0;
                }
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
                                $"Applies To: {string.Join(", ", TargetCategoryObjects.Where(x => x.IsChecked).Select(x => x.CategoryObjectName))}" + Environment.NewLine +
                                $"Tracks Parameter : {TrackingParameterObject.ParameterObjectName}" + Environment.NewLine +
                                $"Regex String: {RegexString}" + Environment.NewLine +
                                $"Writes To : {OutputParameterObject.ParameterObjectName}" + Environment.NewLine +
                                $"Created By: {CreatedBy}" + Environment.NewLine +
                                $"Created At: {DateTimeCreated}";
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

        public bool IsStagingRule { get; set; } = false;
        public string DateTimeCreated { get; set; } = DateTime.Now.ToString("r");
        public string CreatedBy { get; set; } = Environment.UserName;
        public string RuleGuid { get; private set; }

        public RegularUpdater RegularUpdater { get; set; } = new RegularUpdater(RegularApp.RevitApplication.ActiveAddInId);

        public static RegexRule Create(string documentGuid, string guid = null)
        {
            RegexRule regexRule = new RegexRule
            {
                // If given a GUID, a rule is being recreated from memory, otherwise a new rule is being created
                RuleGuid = guid ?? Guid.NewGuid().ToString(),
                TargetCategoryObjects = CategoryUtils.GetInitialCategories(documentGuid)
            };
            return regexRule;
        }

        public static void Save(string documentGuid, RegexRule regexRule)
        {
            // Saves rule to static cache and ExtensibleStorage
            RegularApp.RegexRuleCacheService.AddRule(documentGuid, regexRule);
            RegularApp.DmUpdaterCacheService.AddAndRegisterUpdater(documentGuid, regexRule);
            ExtensibleStorageUtils.SaveRegexRuleToExtensibleStorage(documentGuid, regexRule);

            // TODO: Check this rule is created as we want
            ParameterUtils.CreateProjectParameter(documentGuid, regexRule);
        }

        public static void Delete(string documentGuid, RegexRule regexRule)
        {
            // Deleting the cached RegexRule, associated DataStorage object, cached DmUpdater, UpdaterRegistry object and Trigger
            RegularApp.RegexRuleCacheService.RemoveRule(documentGuid, regexRule.RuleGuid);
            RegularApp.DmUpdaterCacheService.RemoveAndDeRegisterUpdater(documentGuid, regexRule);
            ExtensibleStorageUtils.DeleteRegexRuleFromExtensibleStorage(documentGuid, regexRule.RuleGuid);
        }

        public static void Update(string documentGuid, RegexRule existingRegexRule, RegexRule newRegexRule)
        {
            // Takes a newly-generated RegexRule object and sets an existing rules values to match
            // To be used when updating an existing rule from the Rule Editor

            existingRegexRule.RuleName = newRegexRule.RuleName;
            existingRegexRule.TargetCategoryObjects = newRegexRule.TargetCategoryObjects;
            existingRegexRule.TrackingParameterObject = newRegexRule.TrackingParameterObject;
            existingRegexRule.OutputParameterObject = newRegexRule.OutputParameterObject;
            existingRegexRule.MatchType = newRegexRule.MatchType;
            existingRegexRule.RegexRuleParts = newRegexRule.RegexRuleParts;
            existingRegexRule.RegexString = newRegexRule.RegexString;
            existingRegexRule.IsFrozen = newRegexRule.IsFrozen;
            existingRegexRule.RegularUpdater = newRegexRule.RegularUpdater;

            // Need to check if existingRegexRule is in ExtensibleStorage or not.
            ExtensibleStorageUtils.UpdateRegexRuleInExtensibleStorage(documentGuid, existingRegexRule.RuleGuid, newRegexRule);
            DmTriggerUtils.UpdateTrigger(documentGuid, existingRegexRule);
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
            
            // Returns a copy of an existing RegexRule, but with a new GUID
            RegexRule duplicateRegexRule = Create(documentGuid);
            if (isStagingRule) duplicateRegexRule.IsStagingRule = true;
            // If we're duplicating a rule we need a new name, but if it's a temporary staging rule we don't
            // need to ensure the name and GUID is unique
            duplicateRegexRule.RuleName = isStagingRule ? sourceRegexRule.RuleName : GenerateRegexRuleDuplicateName();
            duplicateRegexRule.TargetCategoryObjects = sourceRegexRule.TargetCategoryObjects;
            duplicateRegexRule.TrackingParameterObject = sourceRegexRule.TrackingParameterObject;
            duplicateRegexRule.OutputParameterObject = new ParameterObject();
            duplicateRegexRule.MatchType = sourceRegexRule.MatchType;
            duplicateRegexRule.RegexRuleParts = sourceRegexRule.RegexRuleParts;
            duplicateRegexRule.RegexString = sourceRegexRule.RegexString;
            duplicateRegexRule.IsFrozen = sourceRegexRule.IsFrozen;
            duplicateRegexRule.RegularUpdater = sourceRegexRule.RegularUpdater;
            return duplicateRegexRule;
        }
       
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
