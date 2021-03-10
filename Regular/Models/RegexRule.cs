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
                toolTipString = $"Rule Name: {RuleName}" + Environment.NewLine +
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

        public bool IsStagingRule { get; set; }
        public string DateTimeCreated { get; set; } = DateTime.Now.ToString("r");
        public string CreatedBy { get; set; } = Environment.UserName;
        public string RuleGuid { get; private set; }

        public RegularUpdater RegularUpdater { get; set; } = new RegularUpdater(RegularApp.RevitApplication.ActiveAddInId);

        public static RegexRule Create(string documentGuid, string regexRuleGuid = null)
        {
            RegexRule regexRule = new RegexRule
            {
                // If given a GUID, a rule is being recreated from memory, otherwise a new rule is being created
                RuleGuid = regexRuleGuid ?? Guid.NewGuid().ToString(),
                TargetCategoryObjects = CategoryUtils.GetInitialCategories(documentGuid)
            };
            return regexRule;
        }

        public static void Save(string documentGuid, RegexRule regexRule)
        {
            ParameterUtils.CreateProjectParameter(documentGuid, regexRule);
            RegularApp.RegexRuleCacheService.AddRule(documentGuid, regexRule);
            RegularApp.DmUpdaterCacheService.AddAndRegisterUpdater(documentGuid, regexRule);
            ExtensibleStorageUtils.SaveRegexRuleToExtensibleStorage(documentGuid, regexRule);
        }

        public static void Delete(string documentGuid, RegexRule regexRule)
        {
            ParameterUtils.DeleteProjectParameter(documentGuid, regexRule);
            RegularApp.RegexRuleCacheService.RemoveRule(documentGuid, regexRule.RuleGuid);
            RegularApp.DmUpdaterCacheService.RemoveAndDeRegisterUpdater(documentGuid, regexRule);
            ExtensibleStorageUtils.DeleteRegexRuleFromExtensibleStorage(documentGuid, regexRule.RuleGuid);
        }

        public static RegexRule Update(string documentGuid, RegexRule existingRegexRule, RegexRule stagingRegexRule)
        {
            ParameterUtils.UpdateProjectParameter(documentGuid, existingRegexRule, stagingRegexRule);
            
            // We copy all properties over from the staging rule to the existing rule
            existingRegexRule = DeepCopyRegexRule(stagingRegexRule);
            
            RegularApp.RegexRuleCacheService.UpdateRule(documentGuid, existingRegexRule);
            RegularApp.DmUpdaterCacheService.UpdaterUpdater(documentGuid, existingRegexRule);
            ExtensibleStorageUtils.UpdateRegexRuleInExtensibleStorage(documentGuid, existingRegexRule.RuleGuid, stagingRegexRule);
            DmTriggerUtils.UpdateTrigger(documentGuid, existingRegexRule);

            return existingRegexRule;
        }

        public static RegexRule DeepCopyRegexRule(RegexRule ruleToCopy)
        {
            RegularUpdater regularUpdater = ruleToCopy.RegularUpdater;
            string ruleGuid = ruleToCopy.RuleGuid;
            ruleToCopy = SerializationUtils.DeepCopyObject(ruleToCopy);
            // Deep copy has issues because it instantites new RegularUpdater objects
            // whereas we just want a reference to the original object when we update it
            ruleToCopy.RuleGuid = ruleGuid;
            ruleToCopy.RegularUpdater = regularUpdater;
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
            
            if (isStagingRule)
            {
                duplicateRegexRule.RegularUpdater = sourceRegexRule.RegularUpdater;
            }
            else
            {
                duplicateRegexRule.RuleGuid = Guid.NewGuid().ToString();
                duplicateRegexRule.RuleName = GenerateRegexRuleDuplicateName();
                // Newly-duplicated rules that aren't staging rules will need to target a new, and different, parameter
                duplicateRegexRule.OutputParameterObject = new ParameterObject();
                duplicateRegexRule.RegularUpdater = new RegularUpdater(RegularApp.RevitApplication.ActiveAddInId);
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
