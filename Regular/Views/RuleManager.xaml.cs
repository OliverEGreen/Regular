using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using Regular.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

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
            RegexRulesListBox.ItemsSource = regexRules;
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

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            List<Category> categoriesList = new List<Category>();
            categoriesList.Add(Utilities.GetCategoryFromBuiltInCategory(_doc, BuiltInCategory.OST_Walls)); //Testing placeholder for now
            categoriesList.Add(Utilities.GetCategoryFromBuiltInCategory(_doc, BuiltInCategory.OST_Doors)); //Testing placeholder for now
            CategorySet categorySet = Utilities.CreateCategorySetFromListOfCategories(_doc, _app, categoriesList);
            
            Utilities.CreateProjectParameter(_doc, _app, "RegularTestParameter", ParameterType.Text, categorySet, BuiltInParameterGroup.INVALID, true);
        }

        private void ButtonEditRule_Click(object sender, RoutedEventArgs e)
        {
            RegexRule testRegexRule = new RegexRule("Test Rule", Utilities.GetCategoryFromBuiltInCategory(_doc, BuiltInCategory.OST_Doors), null, null);
            RuleEditor ruleEditor = new RuleEditor(_doc, _app, testRegexRule);
            ruleEditor.ShowDialog();
        }
    }
}
