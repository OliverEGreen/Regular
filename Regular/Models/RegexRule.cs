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
            return new RegexRule
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

                ToolTip = "",
                DateTimeCreated = DateTime.Now.ToString("r"),
                CreatedBy = Environment.UserName
            };
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
            
            return duplicateRegexRule;
        }
       
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
