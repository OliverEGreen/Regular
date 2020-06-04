using System.Windows;
using Autodesk.Revit.DB;
using System.Windows.Controls;
using Regular.Services;
using Regular.Models;
using Regular;
using System.Collections.ObjectModel;

namespace Regular.Views
{
    public partial class RuleManager : Window
    {
        public static Document Document { get; set; }
        public static string DocumentGuid { get; set; }
        
        public RuleManager(string documentGuid)
        {
            InitializeComponent();
            DocumentGuid = documentGuid;
            ListBoxRegexRules.ItemsSource = RegexRules.AllRegexRules[documentGuid];
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
        }

        private void ButtonAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            RuleEditor ruleEditor = new RuleEditor(DocumentServices.GetRevitDocumentGuid(Document));
            ruleEditor.Closed += RuleEditor_Closed;
            ruleEditor.ShowDialog();
        }
        private void EditRegexRuleButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string regexRuleGuid = ((RegexRule)button.DataContext).Guid;
            RuleEditor ruleEditor = new RuleEditor(DocumentServices.GetRevitDocumentGuid(Document), regexRuleGuid);
            ruleEditor.ShowDialog();
        }
        private void DeleteRegexRuleButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add in confirmation button before rule gets deleted forever
            Button button = sender as Button;
            string regexRuleGuid = ((RegexRule)button.DataContext).Guid;

            // When deleting a rule we must delete both the cached RegexRule and the ES DataStorage object
            RegexRuleManager.DeleteRegexRule(DocumentGuid, regexRuleGuid);
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
            Button button = sender as Button;
            RegexRule regexRule = (RegexRule)button.DataContext;
            ObservableCollection<RegexRule> RegexRules = Regular.RegexRules.AllRegexRules[DocumentGuid];
            int index = RegexRules.IndexOf(regexRule);

            if (index > 0)
            {
                RegexRules.RemoveAt(index);
                RegexRules.Insert(index - 1, regexRule);
            }
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRule regexRule = (RegexRule)button.DataContext;
            ObservableCollection<RegexRule> RegexRules = Regular.RegexRules.AllRegexRules[DocumentGuid];
            int index = RegexRules.IndexOf(regexRule);

            if (index < Regular.RegexRules.AllRegexRules[DocumentGuid].Count)
            {
                RegexRules.RemoveAt(index);
                RegexRules.Insert(index + 1, regexRule);
            }
        }
    }
}
