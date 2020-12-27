using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Regular.Enums;

namespace Regular.Models
{
    public interface IRegexRulePart : INotifyPropertyChanged
    {
        // Most importantly, the kind of rule we're dealing with
        RuleType RuleType { get; set; }

        // The name of the Rule Type being shown in the RulePartsListBox, e.g. Any Digit
        string RuleTypeName { get; set; }

        // Visible, unless we're dealing with a Custom Text rule type in which case we show an input its place.
        Visibility RuleTypeNameVisibility { get; set; }

        // Only visible if we're dealing with a Custom Text type, otherwise invisible
        Visibility RawUserInputTextBoxVisibility { get; set; }

        // Either shows as 'Case Sensitive' or 'UPPER CASE', 'Any Case' or 'lower case'
        // Depending on the rule type we're dealing with. Doesn't show for Any Digit.
        // Either shows Ab3/AB3 etc for case toggling or 'Edit' for Custom Text or Option Set
        string ButtonControlDisplayText { get; set; }

        // Button control which can't be pressed for the Any Digit rule type
        bool IsButtonControlEnabled { get; set; }

        // Raw user input value is only required for Custom Text
        string RawUserInputValue { get; set; }

        // The various options entered in an Option Set
        ObservableCollection<OptionObject> Options { get; set; }

        // Can apply to any rule part, modified via checkbox
        bool IsOptional { get; set; }

        // Any Digit cannot be case sensitive, Any Letter and Alphanumeric have this set
        // generically via the edit button toggle. 
        bool IsCaseSensitive { get; set; }

        // Only Custom Text and Option Set can be case sensitive as per the user's exact inputs
        // This is only visible for those two rule types
        Visibility CaseSensitiveCheckboxVisibility { get; set; }

        // Only Custom Text can take raw user input via the UI, otherwise we show the RuleTypeName in its place
        string CaseSensitiveDisplayString { get; set; }

        // Takes inputs from the button control when it's toggling cases for Any Letter or Alphanumeric
        CaseSensitivity CaseSensitivityMode { get; set; }
    }
}