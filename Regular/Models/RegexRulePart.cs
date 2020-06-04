using System.ComponentModel;
using System.Windows;

namespace Regular.Models
{
    public class RegexRulePart : INotifyPropertyChanged
    {
        public RuleTypes RuleType { get; }
        public string RuleTypeDisplayText { get; set; } = "Rule";
        public string EditButtonDisplayText { get; set; } = "...";
        public bool IsOptional { get; set; } = false;
        public bool IsCaseSensitive { get; set; } = true;
        public string CaseSensitiveDisplayString { get; set; } = "Any Case";
        public bool IsEditable { get; set; } = true;
        public string RawUserInputValue { get; set; } = "";
        Visibility CaseSensitiveVisibility { get; set; }

        // Our default constructor for newly-created RegexRuleParts
        public RegexRulePart(RuleTypes ruleType)
        {
            RuleType = ruleType;
            IsEditable = RuleType == RuleTypes.FreeText || RuleType == RuleTypes.SelectionSet ? true : false;
            CaseSensitiveVisibility = RuleType == RuleTypes.AnyDigit ? Visibility.Visible : Visibility.Hidden;
        }
        // Our detailed constructor for recreating stored RegexRuleParts that were loaded from ExtensibleStorage
        public RegexRulePart(RuleTypes ruleType, bool isOptional, bool isCaseSensitive, bool requiresUserInput, string rawUserInputValue)
        {
            RuleType = ruleType;
            IsOptional = isOptional;
            IsCaseSensitive = isCaseSensitive;
            IsEditable = requiresUserInput;
            RawUserInputValue = rawUserInputValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
}
