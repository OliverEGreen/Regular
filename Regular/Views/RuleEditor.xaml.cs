using Regular.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Regular.Views
{
    public partial class RuleEditor : Window
    {
        public RuleEditor(RegexRule regexRule)
        {
            InitializeComponent();
            ObservableCollection<RegexRulePart> myTestRegexRuleParts = new ObservableCollection<RegexRulePart>();
            MYTESTBLOCK.Text = "TEST";
            RulePartsListBox.ItemsSource = myTestRegexRuleParts;
            myTestRegexRuleParts.Add(new RegexRulePart("Any Letter", RuleTypes.AnyLetter, false));
            myTestRegexRuleParts.Add(new RegexRulePart("Underscore", RuleTypes.Underscore, true));
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
    }
}
