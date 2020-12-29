using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Regular.Enums;
using Regular.Services;

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
        

        public static RegexRule Create(string documentGuid, string guid = null)
        {
            return new RegexRule()
            {
                // If given a GUID, a rule is being recreated from memory, otherwise a new rule is being created
                RuleGuid = guid ?? Guid.NewGuid().ToString(),
                RuleName = "",
                TargetCategoryObjects = CategoryServices.GetInitialCategories(documentGuid),
                TrackingParameterObject = new ParameterObject { ParameterObjectId = -1, ParameterObjectName = "" },
                OutputParameterObject = new ParameterObject { ParameterObjectId = -1, ParameterObjectName = "" },
                MatchType = MatchType.ExactMatch,
                RegexRuleParts = new ObservableCollection<IRegexRulePart>(),
                RegexString = "",
                IsFrozen = false,

                ToolTip = "",
                DateTimeCreated = DateTime.Now.ToString("r"),
                CreatedBy = Environment.UserName
            };
        }
        public static void Save(string documentGuid, RegexRule regexRule)
        {
            RegexRuleCache.AllRegexRules[documentGuid].Add(regexRule);
            ExtensibleStorageServices.SaveRegexRuleToExtensibleStorage(documentGuid, regexRule);
            DmTriggerServices.AddTrigger(documentGuid, regexRule);
            
            // TODO: Check this rule is created as we want
            ParameterServices.CreateProjectParameter(documentGuid, regexRule.OutputParameterObject.ParameterObjectName, regexRule.TargetCategoryObjects);
        }
        public static RegexRule Duplicate(string documentGuid, RegexRule sourceRegexRule)
        {
            // Helper method to ensure duplicate rules always have a unique name
            string GenerateRegexRuleDuplicateName()
            {
                List<string> documentRegexRuleNames = GetDocumentRegexRules(documentGuid).Select(x => x.RuleName).ToList();
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
            
            return duplicateRegexRule;
        }
        
        public static ObservableCollection<RegexRule> GetDocumentRegexRules(string documentGuid)
        {
            return RegexRuleCache.AllRegexRules.ContainsKey(documentGuid) ? RegexRuleCache.AllRegexRules[documentGuid] : null;
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

            // Need to check if existingRegexRule is in ExtensibleStorage or not.
            ExtensibleStorageServices.UpdateRegexRuleInExtensibleStorage(documentGuid, existingRegexRule.RuleGuid, newRegexRule);
            
            DmTriggerServices.UpdateAllTriggers(documentGuid);
        }
        public static void Delete(string documentGuid, string regexRuleGuid)
        {
            // Deletes a RegexRule from the document's static cache
            if (!RegexRuleCache.AllRegexRules.ContainsKey(documentGuid)) return;
            ObservableCollection<RegexRule> documentRegexRules = GetDocumentRegexRules(documentGuid);
            RegexRule regexRule = documentRegexRules.FirstOrDefault(x => x.RuleGuid == regexRuleGuid);
            if (regexRule != null) documentRegexRules.Remove(regexRule);

            DmTriggerServices.DeleteTrigger(documentGuid, regexRule);
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
