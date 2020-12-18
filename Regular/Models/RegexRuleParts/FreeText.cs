using System.ComponentModel;
using System.Windows;
using Regular.Enums;
using Regular.Utilities;

namespace Regular.Models.RegexRuleParts
{
    class FreeText : IRegexRulePart
    {
        private string displayText;
        private string editButtonDisplayText;
        private bool isCaseSensitive;
        private Visibility caseSensitiveCheckboxVisibility;
        private string caseSensitiveDisplayString;

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

        public bool IsEditButtonEnabled { get; set; }
        public CaseSensitivity CaseSensitivityMode { get; set; }
        public RuleType RuleType { get; set; }
        public FreeText()
        {
            DisplayText = "Free Text";
            RawUserInputValue = "";
            IsOptional = false;
            IsCaseSensitive = false;
            CaseSensitiveCheckboxVisibility = Visibility.Visible;
            IsEditButtonEnabled = true;
            CaseSensitivityMode = CaseSensitivity.None;
            CaseSensitiveDisplayString = "Case Sensitive";
            EditButtonDisplayText = "Edit";
            RuleType = RuleType.FreeText;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
