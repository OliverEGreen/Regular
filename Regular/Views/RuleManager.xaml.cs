using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Regular.Models;
using Regular.Views;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB;

namespace Regular.Views
{
    public partial class RuleManager : Window
    {
        public static Document _doc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application _app { get; set; }

        public RuleManager(ObservableCollection<RegexRule> regexRules, Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            InitializeComponent();
            RegexRulesListBox.ItemsSource = regexRules;
            //Setting properties for forms to access document by
            _doc = doc;
            _app = app;
            regexRules.Add(new RegexRule("Dummy Rule 1", null, null, null));
            regexRules.Add(new RegexRule("Dummy Rule 2", null, null, null));
        }

        private void ButtonAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            RegexRule myTestRegexRule = new RegexRule("anotherrule", null, null, null);
            RuleEditor ruleEditor = new RuleEditor(myTestRegexRule);
            ruleEditor.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            List<Category> categoriesList = new List<Category>();
            categoriesList.Add(Utilities.GetCategoryFromBuiltInCategory(_doc, BuiltInCategory.OST_Walls));
            categoriesList.Add(Utilities.GetCategoryFromBuiltInCategory(_doc, BuiltInCategory.OST_Doors));
            CategorySet categorySet = Utilities.CreateCategorySetFromListOfCategories(_doc, _app, categoriesList);
            
            Utilities.CreateProjectParameter(_doc, _app, "RegularTestParameter", ParameterType.Text, categorySet, BuiltInParameterGroup.INVALID, true);
        }
    }
}
