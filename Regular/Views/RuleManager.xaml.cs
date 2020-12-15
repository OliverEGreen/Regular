using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Views
{
    public partial class RuleManager
    {
        public static Document Document { get; set; }
        public static string DocumentGuid { get; set; }
        
        public RuleManager(string documentGuid)
        {
            InitializeComponent();
            DataContext = new RuleManagerViewModel();
            DocumentGuid = documentGuid;
            ListBoxRegexRules.ItemsSource = RegexRules.AllRegexRules[documentGuid];
            Document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);

            // Gives us the ability to close the window with the Esc kay
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
        }

        private void ButtonAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            // No second argument is provided, we'll work with a brand new rule
            RuleEditor ruleEditor = new RuleEditor(DocumentGuidServices.GetDocumentGuidFromExtensibleStorage(Document));
            
            // When the editor closes, the manager will automatically close as well
            ruleEditor.Closed += RuleEditor_Closed;
            ruleEditor.ShowDialog();
        }
        private void ReorderUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRegexRules.Items.Count < 1) return;
            RegexRule regexRule = ListBoxRegexRules.SelectedItem as RegexRule;
            ObservableCollection<RegexRule> regexRules = RegexRules.AllRegexRules[DocumentGuid];
            int index = regexRules.IndexOf(regexRule);

            if (index <= 0) return;
            regexRules.RemoveAt(index);
            regexRules.Insert(index - 1, regexRule);
            ListBoxRegexRules.Focus();
            ListBoxRegexRules.SelectedItem = regexRule;
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRegexRules.Items.Count < 1) return;
            RegexRule regexRule = ListBoxRegexRules.SelectedItem as RegexRule;
            ObservableCollection<RegexRule> regexRules = RegexRules.AllRegexRules[DocumentGuid];
            int index = regexRules.IndexOf(regexRule);

            if (index >= RegexRules.AllRegexRules[DocumentGuid].Count) return;
            regexRules.RemoveAt(index);
            regexRules.Insert(index + 1, regexRule);
            ListBoxRegexRules.Focus();
            ListBoxRegexRules.SelectedItem = regexRule;
        }
        private void ButtonDuplicateRule_OnClick(object sender, RoutedEventArgs e)
        {
            // Duplicates an existing rule for easier rule creation
            RuleEditor ruleEditor = new RuleEditor
            (
                DocumentGuidServices.GetDocumentGuidFromExtensibleStorage(Document),
                // We create a copy of the rule. The editor will work with a staging copy but that's fine
                // As this rule will get created anew when it's submitted
                RegexRule.Duplicate(DocumentGuid, (RegexRule)ListBoxRegexRules.SelectedItem)
            );
            ruleEditor.ShowDialog();
        }
        private void ButtonStopStartRule_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListBoxRegexRules.Items.Count < 1) return;
            RegexRule regexRule = ListBoxRegexRules.SelectedItem as RegexRule;
            regexRule.IsFrozen = !regexRule.IsFrozen;
            ListBoxRegexRules.Focus();
            ListBoxRegexRules.SelectedItem = regexRule;
        }
        private void EditRegexRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            // We open up the editor with the existing rule
            RuleEditor ruleEditor = new RuleEditor
            (
                DocumentGuidServices.GetDocumentGuidFromExtensibleStorage(Document),
                (RegexRule)button.DataContext
            );
            ruleEditor.ShowDialog();
        }
        private void DeleteRegexRuleButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add in confirmation button before rule gets deleted forever
            Button button = sender as Button;
            string regexRuleGuid = ((RegexRule)button.DataContext).RuleGuid;

            // Deleting both the cached RegexRule and the associated DataStorage object
            RegexRule.Delete(DocumentGuid, regexRuleGuid);
            ExtensibleStorageServices.DeleteRegexRuleFromExtensibleStorage(DocumentGuid, regexRuleGuid);
        }
        private void RegexRulesScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ListBoxRegexRules_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            ButtonMoveRulePartUp.IsEnabled = false;
            ButtonMoveRulePartDown.IsEnabled = false;
            ButtonDuplicateRule.IsEnabled = false;
            ButtonStopStartRule.IsEnabled = false;
        }
        private void ListBoxRegexRules_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RegexRule regexRule = ((ListBox)sender).SelectedItem as RegexRule;
            ObservableCollection<RegexRule> regexRules = RegexRules.AllRegexRules[DocumentGuid];
            int index = regexRules.IndexOf(regexRule);
            ButtonMoveRulePartUp.IsEnabled = index != 0;
            ButtonMoveRulePartDown.IsEnabled = index != regexRules.Count - 1;
            ButtonDuplicateRule.IsEnabled = true;
            ButtonStopStartRule.IsEnabled = true;
        }
        private void RuleEditor_Closed(object sender, System.EventArgs e) => Activate();
        private void ButtonClose_Click(object sender, RoutedEventArgs e) => Close();

    }
}
