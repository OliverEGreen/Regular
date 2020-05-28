using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using Regular.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.ExtensibleStorage;
using System.Windows.Controls;
using System.Linq;
using Autodesk.Revit.UI;
using Regular.Services;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace Regular.Views
{
    public partial class RuleManager : Window
    {
        public static Document Document { get; set; }
        public static Application Application { get; set; }

        ObservableCollection<RegexRule> AllRegexRules = new ObservableCollection<RegexRule>();

        public RuleManager(Document document)
        {
            InitializeComponent();
            Application = RegularApp.RevitApplication;
            Document = document;
            
            if (Utilities.LoadRegexRulesFromExtensibleStorage(Document, Application) != null) { AllRegexRules = Utilities.LoadRegexRulesFromExtensibleStorage(Document, Application); }
            else { AllRegexRules = new ObservableCollection<RegexRule>(); }
            RulesListBox.ItemsSource = AllRegexRules;
        }

        private void ButtonAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            RuleEditor ruleEditor = new RuleEditor(DocumentServices.GetRevitDocumentGuid(Document));
            ruleEditor.Closed += RuleEditor_Closed;
            ruleEditor.ShowDialog();
        }

        private void RuleEditor_Closed(object sender, System.EventArgs e)
        {
            AllRegexRules.Clear();
            foreach(RegexRule regexRule in Utilities.LoadRegexRulesFromExtensibleStorage(Document, Application))
            {
                AllRegexRules.Add(regexRule);
            }
            RulesListBox.Items.Refresh();
            Activate();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void ButtonEditRule_Click(object sender, RoutedEventArgs e)
        {
            RegexRule testRegexRule = new RegexRule("Test Rule", "Doors", null, null);
            RuleEditor ruleEditor = new RuleEditor(DocumentServices.GetRevitDocumentGuid(Document));
            ruleEditor.ShowDialog();
        }

        private void DeleteRegexRulePartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RegexRulesScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
