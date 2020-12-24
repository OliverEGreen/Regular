using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Regular.Enums;

namespace Regular.Models.RegexRuleParts
{
    internal class CustomText : IRegexRulePart
    {
        private Visibility caseSensitiveCheckboxVisibility;
        private string caseSensitiveDisplayString;
        private string displayText;
        private string editButtonDisplayText;
        private bool isCaseSensitive;
        private ObservableCollection<string> options;
        private Visibility rawUserInputTextBoxVisibility;
        private Visibility ruleTypeNameVisibility;
        private string rawUserInputValue;
        private bool isOptional;
        private bool isButtonControlEnabled;
        private CaseSensitivity caseSensitivityMode;
        private RuleType ruleType;

        public CustomText()
        {
            RuleTypeName = "Custom Text";
            RawUserInputValue = "";
            IsOptional = false;
            IsCaseSensitive = true;
            CaseSensitiveCheckboxVisibility = Visibility.Visible;
            IsButtonControlEnabled = true;
            CaseSensitivityMode = CaseSensitivity.None;
            CaseSensitiveDisplayString = "Case Sensitive";
            ButtonControlDisplayText = "Edit";
            RuleType = RuleType.CustomText;
            RawUserInputTextBoxVisibility = Visibility.Visible;
            RuleTypeNameVisibility = Visibility.Collapsed;
            Options = new ObservableCollection<string>();
        }

        public Visibility RawUserInputTextBoxVisibility
        {
            get => rawUserInputTextBoxVisibility;
            set => rawUserInputTextBoxVisibility = value;
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

        public Visibility RuleTypeNameVisibility
        {
            get => ruleTypeNameVisibility;
            set => ruleTypeNameVisibility = value;
        }

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

        public string RawUserInputValue
        {
            get => rawUserInputValue;
            set => rawUserInputValue = value;
        }

        public ObservableCollection<string> Options
        {
            get => options;
            set
            {
                options = value;
                NotifyPropertyChanged("Options");
            }
        }

        public bool IsOptional
        {
            get => isOptional;
            set => isOptional = value;
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

        public bool IsButtonControlEnabled
        {
            get => isButtonControlEnabled;
            set => isButtonControlEnabled = value;
        }

        public CaseSensitivity CaseSensitivityMode
        {
            get => caseSensitivityMode;
            set => caseSensitivityMode = value;
        }

        public RuleType RuleType
        {
            get => ruleType;
            set => ruleType = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}