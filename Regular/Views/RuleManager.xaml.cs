using System.Windows;
using Autodesk.Revit.DB;
using System.Windows.Controls;
using Regular.Services;
using Regular.Models;
using Regular;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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
            RegexRule regexRule = new RegexRule()
            {
                Name = "Name",
                OutputParameterName = "OutputParameterName",
                RegexRuleParts = new ObservableCollection<RegexRulePart>(),
                RegexString = "RegexString",
                TargetCategoryNames = new List<string>(),
                ToolTip = "ToolTip",
                TrackingParameterName = "TrackingParameterName"
            };
            RuleEditor ruleEditor = new RuleEditor(DocumentServices.GetRevitDocumentGuid(Document), regexRule);
            ruleEditor.Closed += RuleEditor_Closed;
            ruleEditor.ShowDialog();
        }
        private void EditRegexRuleButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRule regexRule = ((RegexRule)button.DataContext);
            RuleEditor ruleEditor = new RuleEditor(DocumentServices.GetRevitDocumentGuid(Document), regexRule);
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
