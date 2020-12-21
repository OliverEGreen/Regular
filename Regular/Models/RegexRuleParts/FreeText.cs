using System.ComponentModel;
using System.Windows;
using Regular.Enums;

namespace Regular.Models.RegexRuleParts
{
    internal class FreeText : IRegexRulePart
    {
        private Visibility caseSensitiveCheckboxVisibility;
        private string caseSensitiveDisplayString;
        private string displayText;
        private string editButtonDisplayText;
        private bool isCaseSensitive;

        public FreeText()
        {
            RuleTypeName = "Free Text";
            RawUserInputValue = "";
            IsOptional = false;
            IsCaseSensitive = true;
            CaseSensitiveCheckboxVisibility = Visibility.Visible;
            IsButtonControlEnabled = true;
            CaseSensitivityMode = CaseSensitivity.None;
            CaseSensitiveDisplayString = "Case Sensitive";
            ButtonControlDisplayText = "Edit";
            RuleType = RuleType.FreeText;
            RawUserInputTextBoxVisibility = Visibility.Visible;
            RuleTypeNameVisibility = Visibility.Collapsed;
        }

        public Visibility RawUserInputTextBoxVisibility { get; set; }

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

        Visibility IRegexRulePart.RawUserInputTextBoxVisibility { get; set; }

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