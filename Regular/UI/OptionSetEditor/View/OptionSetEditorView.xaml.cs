using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.OptionSetEditor.ViewModel;

namespace Regular.UI.OptionSetEditor.View
{
    public partial class OptionSetEditorView
    {
        public OptionSetEditorViewModel OptionSetEditorViewModel { get; set; }
        public IRegexRulePart RegexRulePart { get; set; }
        
        public OptionSetEditorView(IRegexRulePart regexRulePart)
        {
            InitializeComponent();
            OptionSetEditorViewModel = new OptionSetEditorViewModel(regexRulePart);
            RegexRulePart = regexRulePart;
            DataContext = OptionSetEditorViewModel;
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            List<string> options = OptionSetEditorViewModel.Options.Select(x => x.OptionValue).ToList();
            RegexRulePart.RawUserInputValue = string.Join(", ", options);
            RegexRulePart.RawUserInputTextBoxVisibility = Visibility.Visible;
            RegexRulePart.RuleTypeNameVisibility = Visibility.Collapsed;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();
        
        private void OptionSetEditorView_OnLoaded(object sender, RoutedEventArgs e) => DataGridOptions.Focus();
    }
}
