using System.ComponentModel;
using Regular.Enums;

namespace Regular.ViewModel
{
    public class RegexRulePart : INotifyPropertyChanged
    {
        private string displayText;
        private string editButtonDisplayText;
        private string rawUserInputValue;
        private string caseSensitiveDisplayString;
        private bool isOptional;
        private bool isCaseSensitive;
        private bool isCaseSensitiveCheckboxVisible;
        private bool isEditable;
        
        public RuleType RuleType { get; }
        public string DisplayText
        {
            get => displayText;
            set
            {
                displayText = value;
                NotifyPropertyChanged("DisplayText");
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
        public string RawUserInputValue
        {
            get => rawUserInputValue;
            set
            {
                rawUserInputValue = value;
                NotifyPropertyChanged("RawUserInputValue");
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
        public bool IsCaseSensitiveCheckboxVisible
        {
            get => isCaseSensitiveCheckboxVisible;
            set
            {
                isCaseSensitiveCheckboxVisible = value;
                NotifyPropertyChanged("IsCaseSensitiveCheckboxVisible");
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

        // Our default constructor for newly-created RegexRuleParts
        public RegexRulePart(RuleType ruleType)
        {
            RuleType = ruleType;
            IsOptional = false;
            IsCaseSensitive = false;
            RawUserInputValue = "";
            IsEditable = true;
            IsCaseSensitiveCheckboxVisible = true;
            
            switch (RuleType)
            {
                case RuleType.AnyDigit:
                    DisplayText = "Any Digit";
                    IsEditable = false;
                    EditButtonDisplayText = "0-9";
                    IsCaseSensitiveCheckboxVisible = false;
                    break;
                case RuleType.AnyCharacter:
                    DisplayText = "Any Character";
                    EditButtonDisplayText = "AB1";
                    CaseSensitiveDisplayString = "UPPER CASE";
                    IsCaseSensitiveCheckboxVisible = false;
                    break;
                case RuleType.AnyLetter:
                    DisplayText = "Any Letter";
                    EditButtonDisplayText = "A-Z";
                    CaseSensitiveDisplayString = "UPPER CASE";
                    IsCaseSensitiveCheckboxVisible = false;
                    break;
                case RuleType.FreeText:
                    DisplayText = "Free Text";
                    EditButtonDisplayText = "Edit";
                    CaseSensitiveDisplayString = "Case Sensitive";
                    break;
                case RuleType.SelectionSet:
                    DisplayText = "Selection Set";
                    EditButtonDisplayText = "Edit";
                    CaseSensitiveDisplayString = "Case Sensitive";
                    break;
            }
        }

        // Our detailed constructor for recreating stored RegexRuleParts that were loaded from ExtensibleStorage
        public RegexRulePart(RuleType ruleType, bool isOptional, bool isCaseSensitive, bool requiresUserInput)
        {
            RuleType = ruleType;
            IsOptional = isOptional;
            IsCaseSensitive = isCaseSensitive;
            IsEditable = requiresUserInput;
            
            switch (RuleType)
            {
                case RuleType.AnyDigit:
                    DisplayText = "Any Digit";
                    IsEditable = false;
                    EditButtonDisplayText = "0-9";
                    break;
                case RuleType.AnyCharacter:
                    DisplayText = "Any Character";
                    IsEditable = true;
                    EditButtonDisplayText = "AB1";
                    break;
                case RuleType.AnyLetter:
                    DisplayText = "Any Letter";
                    IsEditable = true;
                    EditButtonDisplayText = "A-Z";
                    break;
                case RuleType.FreeText:
                    DisplayText = "Free Text";
                    IsEditable = true;
                    EditButtonDisplayText = "Edit";
                    break;
                case RuleType.SelectionSet:
                    DisplayText = "Selection Set";
                    IsEditable = true;
                    EditButtonDisplayText = "Edit";
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
