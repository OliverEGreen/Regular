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

namespace Regular.Views
{
    public partial class RuleManager : Window
    {
        public static Document _doc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application _app { get; set; }

        ObservableCollection<RegexRule> AllRegexRules = new ObservableCollection<RegexRule>();

        public RuleManager(Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            InitializeComponent();
            _doc = doc;
            _app = app;
            if (Utilities.LoadRegexRulesFromExtensibleStorage(_doc, _app) != null) { AllRegexRules = Utilities.LoadRegexRulesFromExtensibleStorage(_doc, _app); }
            else { AllRegexRules = new ObservableCollection<RegexRule>(); }
            RulesListBox.ItemsSource = AllRegexRules;
        }

        private void ButtonAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            RuleEditor ruleEditor = new RuleEditor(_doc, _app);
            ruleEditor.Closed += RuleEditor_Closed;
            ruleEditor.ShowDialog();
        }

        private void RuleEditor_Closed(object sender, System.EventArgs e)
        {
            AllRegexRules.Clear();
            foreach(RegexRule regexRule in Utilities.LoadRegexRulesFromExtensibleStorage(_doc, _app))
            {
                AllRegexRules.Add(regexRule);
            }
            RulesListBox.Items.Refresh();
            Activate();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonEditRule_Click(object sender, RoutedEventArgs e)
        {
            RegexRule testRegexRule = new RegexRule("Test Rule", Utilities.GetCategoryFromBuiltInCategory(_doc, BuiltInCategory.OST_Doors), null, null);
            RuleEditor ruleEditor = new RuleEditor(_doc, _app, testRegexRule);
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
