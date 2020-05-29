﻿using System.Windows;
using Autodesk.Revit.DB;
using System.Windows.Controls;
using Regular.Services;
using Regular.Models;

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
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);

            // All rules found in ExtensibleStorage are loaded on the DocumentOpened event
            RulesListBox.ItemsSource = RegularApp.AllRegexRules[documentGuid];
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
    }
}
