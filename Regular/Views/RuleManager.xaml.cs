using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using Regular.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System.Windows.Controls;

namespace Regular.Views
{
    public partial class RuleManager : Window
    {
        public static Document _doc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application _app { get; set; }

        public RuleManager(ObservableCollection<RegexRule> regexRules, Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            InitializeComponent();
            _doc = doc;
            _app = app;
            RulesListBox.ItemsSource = regexRules;
        }

        private void ButtonAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            RuleEditor ruleEditor = new RuleEditor(_doc, _app);
            ruleEditor.ShowDialog();
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

        }
    }
}
