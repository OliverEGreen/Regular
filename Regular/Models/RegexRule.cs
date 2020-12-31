using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Autodesk.Revit.DB;
using Regular.Enums;
using Regular.Services;
using Regular.Utilities;

namespace Regular.Models
{
    public class RegexRule : INotifyPropertyChanged
    {
        private string ruleName;
        private ObservableCollection<CategoryObject> targetCategoryObjects;
        private ObservableCollection<IRegexRulePart> regexRuleParts;
        private ParameterObject trackingParameterObject;
        private ParameterObject outputParameterObject;
        private string toolTipString;
        private string regexString;
        private MatchType matchType;
        private bool isFrozen;
        private string dateTimeCreated;
        private string createdBy;
        private string ruleGuid;
        private RegularUpdater regularUpdater;
        public string RuleName
        {
            get => ruleName;
            set
            {
                ruleName = value;
                NotifyPropertyChanged("RuleName");
            }
        }

        public string DateTimeCreated
        {
            get => dateTimeCreated;
            private set => dateTimeCreated = value;
        }

        public string CreatedBy
        {
            get => createdBy;
            private set => createdBy = value;
        }

        public string RuleGuid
        {
            get => ruleGuid;
            private set => ruleGuid = value;
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
                                $"Applies To: {String.Join(", ", TargetCategoryObjects.Where(x => x.IsChecked).Select(x => x.CategoryObjectName))}" + Environment.NewLine +
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
        public ObservableCollection<IRegexRulePart> RegexRuleParts
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

        public RegularUpdater RegularUpdater
        {
            get => regularUpdater;
            set => regularUpdater = value;
        }
        
        public static RegexRule Create(string documentGuid, string guid = null)
        {
            RegexRule regexRule = new RegexRule
            {
                // If given a GUID, a rule is being recreated from memory, otherwise a new rule is being created
                RuleGuid = guid ?? Guid.NewGuid().ToString(),
                RuleName = "",
                TargetCategoryObjects = CategoryUtils.GetInitialCategories(documentGuid),
                TrackingParameterObject = new ParameterObject { ParameterObjectId = -1, ParameterObjectName = "" },
                OutputParameterObject = new ParameterObject { ParameterObjectId = -1, ParameterObjectName = "" },
                MatchType = MatchType.ExactMatch,
                RegexRuleParts = new ObservableCollection<IRegexRulePart>(),
                RegexString = "",
                IsFrozen = false,
                RegularUpdater = new RegularUpdater(RegularApp.RevitApplication.ActiveAddInId),
                ToolTip = "",
                DateTimeCreated = DateTime.Now.ToString("r"),
                CreatedBy = Environment.UserName
            };
            return regexRule;
        }

        public static void Save(string documentGuid, RegexRule regexRule)
        {
            // Saves rule to static cache and ExtensibleStorage
            RegularApp.RegexRuleCacheService.AddRule(documentGuid, regexRule);
            RegularApp.DmUpdaterCacheService.AddUpdater(documentGuid, regexRule);
            ExtensibleStorageUtils.SaveRegexRuleToExtensibleStorage(documentGuid, regexRule);

            // TODO: Check this rule is created as we want
            ParameterUtils.CreateProjectParameter(documentGuid, regexRule);
        }

        public static void Delete(string documentGuid, RegexRule regexRule)
        {
            // Deleting the cached RegexRule, associated DataStorage object, cached DmUpdater, UpdaterRegistry object and Trigger
            RegularApp.RegexRuleCacheService.RemoveRule(documentGuid, regexRule.RuleGuid);
            RegularApp.DmUpdaterCacheService.RemoveUpdater(documentGuid, regexRule);
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

        public static RegexRule Duplicate(string documentGuid, RegexRule sourceRegexRule)
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
            duplicateRegexRule.RuleName = GenerateRegexRuleDuplicateName();
            duplicateRegexRule.TargetCategoryObjects = sourceRegexRule.TargetCategoryObjects;
            duplicateRegexRule.TrackingParameterObject = sourceRegexRule.TrackingParameterObject;
            duplicateRegexRule.OutputParameterObject = sourceRegexRule.OutputParameterObject;
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
