using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Regular.Model;
using Regular.Services;
using Regular.ViewModel;

namespace Regular.View
{
    public partial class RuleManager
    {
        public static Document Document { get; set; }
        public static string DocumentGuid { get; set; }
        
        public RuleManager(string documentGuid)
        {
            InitializeComponent();
            DocumentGuid = documentGuid;
            ListBoxRegexRules.ItemsSource = RegexRules.AllRegexRules[documentGuid];
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);

            // Gives us the ability to close the window with the Esc kay
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
        }

        private void ButtonAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            RuleEditor ruleEditor = new RuleEditor(DocumentServices.GetRevitDocumentGuid(Document), RegexRule.Create(DocumentGuid));
            ruleEditor.Closed += RuleEditor_Closed;
            ruleEditor.ShowDialog();
        }
        private void EditRegexRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            RegexRule regexRule = ((RegexRule)button.DataContext);
            RuleEditor ruleEditor = new RuleEditor(DocumentServices.GetRevitDocumentGuid(Document), regexRule);
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
        private void RegexRulesScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e) => Close();
        private void RuleEditor_Closed(object sender, System.EventArgs e) => Activate();
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
        private void ButtonDuplicateRule_OnClick(object sender, RoutedEventArgs e)
        {
            RegexRule regexRule = ListBoxRegexRules.SelectedItem as RegexRule;
            RegexRule.Save(DocumentGuid, RegexRule.Duplicate(DocumentGuid, regexRule));
        }
        private void ListBoxRegexRules_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            ButtonMoveRulePartUp.IsEnabled = false;
            ButtonMoveRulePartDown.IsEnabled = false;
            ButtonDuplicateRule.IsEnabled = false;
            ButtonStopStartRule.IsEnabled = false;
        }
    }
}
