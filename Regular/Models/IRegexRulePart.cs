using System.ComponentModel;
using System.Windows;
using Regular.Enums;

namespace Regular.Models
{
    public interface IRegexRulePart : INotifyPropertyChanged
    {
        string DisplayText { get; set; }
        string EditButtonDisplayText { get; set; }
        string RawUserInputValue { get; set; }
        bool IsOptional { get; set; }
        bool IsCaseSensitive { get; set; }
        Visibility CaseSensitiveCheckboxVisibility{ get; set; }
        string CaseSensitiveDisplayString { get; set; }
        bool IsEditButtonEnabled { get; set; }
        CaseSensitivity CaseSensitivityMode { get; set; }
        RuleType RuleType { get; set; }
    }
}
