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
            string regexRuleGuid = ((RegexRule)button.DataContext).Guid;

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
            if (!(sender is Button button)) return;
            RegexRule regexRule = (RegexRule)button.DataContext;
            ObservableCollection<RegexRule> regexRules = RegexRules.AllRegexRules[DocumentGuid];
            int index = regexRules.IndexOf(regexRule);

            if (index <= 0) return;
            regexRules.RemoveAt(index);
            regexRules.Insert(index - 1, regexRule);
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            RegexRule regexRule = (RegexRule)button.DataContext;
            ObservableCollection<RegexRule> regexRules = RegexRules.AllRegexRules[DocumentGuid];
            int index = regexRules.IndexOf(regexRule);

            if (index >= RegexRules.AllRegexRules[DocumentGuid].Count) return;
            regexRules.RemoveAt(index);
            regexRules.Insert(index + 1, regexRule);
        }
    }
}
