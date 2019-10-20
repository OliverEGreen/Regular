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
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Regular.Views
{
    public partial class RuleManager : Window
    {
        public RuleManager(ObservableCollection<RegexRule> regexRules)
        {
            InitializeComponent();
            RegexRulesListBox.ItemsSource = regexRules;
            regexRules.Add(new RegexRule("mycoolrule", null, null, null));
            regexRules.Add(new RegexRule("anotherrule", null, null, null));
        }

        private void ButtonAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            RegexRule myTestRegexRule = new RegexRule("anotherrule", null, null, null);
            RuleEditor ruleEditor = new RuleEditor(myTestRegexRule);
            ruleEditor.ShowDialog();
        }
    }
}
