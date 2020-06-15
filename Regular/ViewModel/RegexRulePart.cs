using System.ComponentModel;
using Regular.Enums;
using Regular.Model;

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
        
        public RuleType RuleType { get; }
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
        public RegexRulePart(RuleType ruleType)
        {
            RuleType = ruleType;
            IsOptional = false;
            IsCaseSensitive = false;
            RawUserInputValue = "";

            switch (RuleType)
            {
                case RuleType.AnyDigit:
                    RuleTypeDisplayText = "Any Digit";
                    IsEditable = false;
                    EditButtonDisplayText = "0-9";
                    break;
                case RuleType.AnyLetter:
                    RuleTypeDisplayText = "Any Letter";
                    IsEditable = true;
                    EditButtonDisplayText = "A-Z";
                    break;
                case RuleType.FreeText:
                    RuleTypeDisplayText = "Free Text";
                    IsEditable = true;
                    EditButtonDisplayText = "...";
                    break;
                case RuleType.SelectionSet:
                    RuleTypeDisplayText = "Selection Set";
                    IsEditable = true;
                    EditButtonDisplayText = "...";
                    break;
            }
        }

        // Our detailed constructor for recreating stored RegexRuleParts that were loaded from ExtensibleStorage
        public RegexRulePart(RuleType ruleType, bool isOptional, bool isCaseSensitive, bool requiresUserInput, string rawUserInputValue)
        {
            RuleType = ruleType;
            IsOptional = isOptional;
            IsCaseSensitive = isCaseSensitive;
            IsEditable = requiresUserInput;
            RawUserInputValue = rawUserInputValue;

            switch (RuleType)
            {
                case RuleType.AnyDigit:
                    RuleTypeDisplayText = "Any Digit";
                    IsEditable = false;
                    EditButtonDisplayText = "0-9";
                    break;
                case RuleType.AnyLetter:
                    RuleTypeDisplayText = "Any Letter";
                    IsEditable = true;
                    EditButtonDisplayText = "A-Z";
                    break;
                case RuleType.FreeText:
                    RuleTypeDisplayText = "Free Text";
                    IsEditable = true;
                    EditButtonDisplayText = "...";
                    break;
                case RuleType.SelectionSet:
                    RuleTypeDisplayText = "Selection Set";
                    IsEditable = true;
                    EditButtonDisplayText = "...";
                    break;
            }
        }   

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
