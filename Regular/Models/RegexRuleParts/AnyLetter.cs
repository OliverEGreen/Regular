using System.ComponentModel;
using System.Windows;
using Regular.Enums;
using Regular.Utilities;

namespace Regular.Models.RegexRuleParts
{
    internal class AnyLetter : IRegexRulePart
    {
        private Visibility caseSensitiveCheckboxVisibility;
        private string caseSensitiveDisplayString;
        private string displayText;
        private string editButtonDisplayText;
        private bool isCaseSensitive;

        public AnyLetter()
        {
            RuleTypeName = "Any Letter";
            RawUserInputValue = "";
            IsOptional = false;
            IsCaseSensitive = false;
            CaseSensitiveCheckboxVisibility = Visibility.Hidden;
            IsButtonControlEnabled = true;
            CaseSensitivityMode = CaseSensitivity.AnyCase;
            CaseSensitiveDisplayString = EnumUtils.GetEnumDescription(CaseSensitivityMode);
            ButtonControlDisplayText = "A-z";
            RuleType = RuleType.AnyLetter;
            RawUserInputTextBoxVisibility = Visibility.Collapsed;
            RuleTypeNameVisibility = Visibility.Visible;
        }

        public string RuleTypeName
        {
            get => displayText;
            set
            {
                displayText = value;
                NotifyPropertyChanged("RuleTypeName");
            }
        }

        public Visibility RuleTypeNameVisibility { get; set; }
        public Visibility RawUserInputTextBoxVisibility { get; set; }

        public string ButtonControlDisplayText
        {
            get => editButtonDisplayText;
            set
            {
                editButtonDisplayText = value;
                NotifyPropertyChanged("ButtonControlDisplayText");
            }
        }

        public string RawUserInputValue { get; set; }
        public bool IsOptional { get; set; }

        public bool IsCaseSensitive
        {
            get => isCaseSensitive;
            set
            {
                isCaseSensitive = value;
                NotifyPropertyChanged("IsCaseSensitive");
            }
        }

        public Visibility CaseSensitiveCheckboxVisibility
        {
            get => caseSensitiveCheckboxVisibility;
            set
            {
                caseSensitiveCheckboxVisibility = value;
                NotifyPropertyChanged("CaseSensitiveCheckboxVisibility");
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

        public bool IsButtonControlEnabled { get; set; }
        public CaseSensitivity CaseSensitivityMode { get; set; }
        public RuleType RuleType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}