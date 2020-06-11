using System.ComponentModel;

namespace Regular.ViewModel
{
    public class RegexRulePart : INotifyPropertyChanged
    {
        private string ruleTypeDisplayText;
        private string editButtonDisplayText;
        private bool isOptional;
        private bool isCaseSensitive;
        private string caseSensitiveDisplayString;
        private bool isEditable;
        private string rawUserInputValue;
        
        public RuleTypes RuleType { get; }
        public string RuleTypeDisplayText
        {
            get => ruleTypeDisplayText;
            set
            {
                ruleTypeDisplayText = value;
                NotifyPropertyChanged("RuleTypeDisplayText");
            }
        }
        public string EditButtonDisplayText
        {
            get => editButtonDisplayText;
            set
            {
                editButtonDisplayText = value;
                NotifyPropertyChanged("EditButtonDisplayText");
            }
        }
        public bool IsOptional
        {
            get => isOptional;
            set
            {
                isOptional = value;
                NotifyPropertyChanged("IsOptional");
            }
        }
        public bool IsCaseSensitive
        {
            get => isCaseSensitive;
            set
            {
                isCaseSensitive = value;
                NotifyPropertyChanged("IsCaseSensitive");
            }
        }
        public string CaseSensitiveDisplayString
        {
            get => caseSensitiveDisplayString;
            set
            {
                caseSensitiveDisplayString = value;
                NotifyPropertyChanged("CaseSensitiveDisplayString");
            }
        }
        public bool IsEditable
        {
            get => isEditable;
            set
            {
                isEditable = value;
                NotifyPropertyChanged("IsEditable");
            }
        }
        public string RawUserInputValue
        {
            get => rawUserInputValue;
            set
            {
                rawUserInputValue = value;
                NotifyPropertyChanged("RawUserInputValue");
            }
        }

        // Our default constructor for newly-created RegexRuleParts
        public RegexRulePart(RuleTypes ruleType)
        {
            RuleType = ruleType;
            IsEditable = RuleType == RuleTypes.FreeText || RuleType == RuleTypes.SelectionSet;
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
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
