using Autodesk.Revit.DB;
using Regular.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections;
using Autodesk.Revit.UI;
using System;

namespace Regular.Views
{
    public partial class RuleEditor : Window
    {
        public ObservableCollection<RegexRulePart> selectedRegexRuleParts = new ObservableCollection<RegexRulePart>();

        public RuleEditor(RegexRule regexRule)
        {
            InitializeComponent();
            
            RulePartsListBox.ItemsSource = selectedRegexRuleParts;

            Categories categories = Regular.RuleManager._doc.Settings.Categories;
            List<string> categoryNames = new List<string>();

            foreach(Category category in categories)
            {
                if(category.AllowsBoundParameters == true)
                {
                    categoryNames.Add(category.Name);
                }
            }
            categoryNames = categoryNames.OrderBy(x => x).ToList();
            ComboBoxInputCategory.Items.Clear();
            ComboBoxInputCategory.ItemsSource = categoryNames;

            //Binding ComboBox to our RuleType enumeration
            ComboBoxInputRulePartType.ItemsSource = Enum.GetValues(typeof(RuleTypes)).Cast<RuleTypes>();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void ComboBoxInputCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Document doc = Regular.RuleManager._doc;
            string selectedCategoryName = ComboBoxInputCategory.SelectedValue.ToString();
            
            //We need the ability to fetch parameters that are bound to the selected category. WIP
        }

        private void AddRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            RuleTypes selectedRuleType = (RuleTypes)ComboBoxInputRulePartType.SelectedItem;
            if(selectedRuleType == RuleTypes.AnyLetter | selectedRuleType == RuleTypes.AnyCharacter | selectedRuleType == RuleTypes.AnyNumber | selectedRuleType == RuleTypes.Anything)
            {
                selectedRegexRuleParts.Add(new RegexRulePart("", (RuleTypes)ComboBoxInputRulePartType.SelectedItem, false));
            }
            else
            {
                selectedRegexRuleParts.Add(new RegexRulePart("Type Here", (RuleTypes)ComboBoxInputRulePartType.SelectedItem, false));
            }
        }
    }
}
