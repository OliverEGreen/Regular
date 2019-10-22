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
using System.Windows.Controls;

namespace Regular.Views
{
    public partial class RuleEditor : Window
    {
        //Helper method to build regex string
        public string GetRegexPartFromRuleType(RegexRulePart regexRulePart)
        {
            switch (regexRulePart.RuleType)
            {
                case RuleTypes.AnyCharacter:
                    return @"\w";
                case RuleTypes.AnyFromSet:
                    //We'll need to break these up somehow
                    return "Test";
                case RuleTypes.AnyLetter:
                    return @"[a-zA-Z]";
                case RuleTypes.AnyNumber:
                    return @"\d";
                case RuleTypes.Anything:
                    return @".";
                case RuleTypes.Dot:
                    return @"\.";
                case RuleTypes.Hyphen:
                    return @"\-";
                case RuleTypes.SpecificCharacter:
                    return $@"[{regexRulePart.RawUserInputValue}]";
                case RuleTypes.SpecificLetter:
                    return $@"[{regexRulePart.RawUserInputValue}]";
                case RuleTypes.SpecificNumber:
                    return $@"[{regexRulePart.RawUserInputValue}]";
                case RuleTypes.Underscore:
                    return @"_";
            }
            return null;
        }

        public ObservableCollection<RegexRulePart> selectedRegexRuleParts = new ObservableCollection<RegexRulePart>();
        public RegexRule RegexRule { get; set; }
        
        //Constructor
        public RuleEditor(RegexRule regexRule)
        {
            InitializeComponent();
            RegexRule = regexRule;
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
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ComboBoxInputCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Document doc = Regular.RuleManager._doc;
            string selectedCategoryName = ComboBoxInputCategory.SelectedValue.ToString();
            
            //We need the ability to fetch parameters that are bound to the selected category. WIP
        }

        private void AddRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            string prefix = selectedRegexRuleParts.Count > 0 ? "Followed by: " : "";
            RuleTypes selectedRuleType = (RuleTypes)ComboBoxInputRulePartType.SelectedItem;

            RegexRulePart newRegexRulePart = new RegexRulePart("'.' (Full Stop)", (RuleTypes)ComboBoxInputRulePartType.SelectedItem, false);
            newRegexRulePart.DisplayString = prefix;

            if (selectedRuleType == RuleTypes.AnyLetter | selectedRuleType == RuleTypes.AnyCharacter | selectedRuleType == RuleTypes.AnyNumber | selectedRuleType == RuleTypes.Anything)
            {
                newRegexRulePart.DisplayString += "Any Digit (0-9)";
                newRegexRulePart.RawUserInputValue = "";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if(selectedRuleType == RuleTypes.Dot)
            {
                newRegexRulePart.DisplayString += "'.' (Full Stop)";
                newRegexRulePart.RawUserInputValue = "";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if (selectedRuleType == RuleTypes.Hyphen)
            {
                newRegexRulePart.DisplayString += "'-' (Hyphen)";
                newRegexRulePart.RawUserInputValue = "";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if (selectedRuleType == RuleTypes.Underscore)
            {
                newRegexRulePart.DisplayString += "'_' (Underscore)";
                newRegexRulePart.RawUserInputValue = "";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
            else
            {
                newRegexRulePart.DisplayString += "User-Defined Value";
                newRegexRulePart.RawUserInputValue = "Type Value Here";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(RegexRulePart regexRulePart in selectedRegexRuleParts)
            {
                RegexRule.RegexString += GetRegexPartFromRuleType(regexRulePart); //Something!! We build the string as we close the editor 
            }

            Close();
            TaskDialog.Show("FOR DEMO", $"Regex is: {RegexRule.RegexString}");
            //We have to add the rules back now
        }

        private void DeleteRegexRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            TaskDialog.Show("FOR DEMO", "This is a real button but it doesn't delete anything yet!");
        }
    }
}
