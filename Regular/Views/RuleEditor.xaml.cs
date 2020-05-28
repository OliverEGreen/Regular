using Autodesk.Revit.DB;
using Regular.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System;
using System.Windows.Controls;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Regular.Services;

namespace Regular.Views
{
    public partial class RuleEditor : Window
    {
        public static Document Document { get; set; }
        public static string DocumentGuid { get; set; }

        // Helper method to build regex string
        public string GetRegexPartFromRuleType(RegexRulePart regexRulePart)
        {
            switch (regexRulePart.RuleType)
            {
                case RuleTypes.AnyCharacter:
                    return @"\w";
                case RuleTypes.AnyFromSet:
                    // We'll need to break these up somehow
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
        public EditorOpeningType EditorOpeningType { get; set; }

        // Constructor for creating a new rule
        public RuleEditor(string documentGuid)
        {
            InitializeComponent();
            
            EditorOpeningType = EditorOpeningType.CreateNewRule;
            Title = "Creating New Rule";
            DocumentGuid = documentGuid;
            
            // Depending on the OpeningType we either need to edit and existing rule
            // In which case we need to take it as an argument and fill out the UI boxes
            // Or we are creating a new rule from scratch
            // In which case we instantiate the new DataObject once the form is filled out and closed
            selectedRegexRuleParts.Clear();
            RulePartsListBox.ItemsSource = selectedRegexRuleParts;

            // We create a new RegexRule placeholder
            RegexRule = new RegexRule(null, null, null, null);

            Categories categories = Regular.RuleManager.Document.Settings.Categories;
            List<string> categoryNames = new List<string>();

            foreach (Category category in categories)
            {
                if (category.AllowsBoundParameters == true)
                {
                    categoryNames.Add(category.Name);
                }
            }
            categoryNames = categoryNames.OrderBy(x => x).ToList();
            ComboBoxInputCategory.Items.Clear();
            ComboBoxInputCategory.ItemsSource = categoryNames;

            // Binding ComboBox to our RuleType enumeration
            ComboBoxInputRulePartType.ItemsSource = Enum.GetValues(typeof(RuleTypes)).Cast<RuleTypes>();
        }

        // Constructor overload for editing existing rules
        public RuleEditor(RegexRule regexRule, string documentGuid)
        {
            InitializeComponent();

            EditorOpeningType = EditorOpeningType.EditExistingRule;
            Title = $"Editing Rule: {regexRule.RuleName}";
            DocumentGuid = documentGuid;
            // Depending on the OpeningType we either need to edit and existing rule
            // In which case we need to take it as an argument and fill out the UI boxes
            // Or we are creating a new rule from scratch
            // In which case we instantiate the new DataObject once the form is filled out and closed
            RegexRule = regexRule;
            RulePartsListBox.ItemsSource = regexRule.RegexRuleParts;

            Categories categories = Regular.RuleManager.Document.Settings.Categories;
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

            // Binding ComboBox to our RuleType enumeration
            ComboBoxInputRulePartType.ItemsSource = Enum.GetValues(typeof(RuleTypes)).Cast<RuleTypes>();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ComboBoxInputCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Document doc = Regular.RuleManager.Document;
            string selectedCategoryName = ComboBoxInputCategory.SelectedValue.ToString();
            
            // We need the ability to fetch parameters that are bound to the selected category. WIP
        }

        private void AddRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            RuleTypes selectedRuleType = (RuleTypes)ComboBoxInputRulePartType.SelectedItem;

            RegexRulePart newRegexRulePart = new RegexRulePart("'.' (Full Stop)", (RuleTypes)ComboBoxInputRulePartType.SelectedItem, false);
            
            if (selectedRuleType == RuleTypes.AnyLetter | selectedRuleType == RuleTypes.AnyCharacter | selectedRuleType == RuleTypes.AnyNumber | selectedRuleType == RuleTypes.Anything)
            {
                newRegexRulePart.DisplayString = "Any Digit";
                newRegexRulePart.RawUserInputValue = "";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if(selectedRuleType == RuleTypes.Dot)
            {
                newRegexRulePart.DisplayString = "Full Stop";
                newRegexRulePart.RawUserInputValue = "";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if (selectedRuleType == RuleTypes.Hyphen)
            {
                newRegexRulePart.DisplayString = "Hyphen";
                newRegexRulePart.RawUserInputValue = "";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if (selectedRuleType == RuleTypes.Underscore)
            {
                newRegexRulePart.DisplayString = "Underscore";
                newRegexRulePart.RawUserInputValue = "";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
            else
            {
                newRegexRulePart.DisplayString = "Specific Character";
                newRegexRulePart.RawUserInputValue = " ";
                selectedRegexRuleParts.Add(newRegexRulePart);
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            string ruleNameInput = TextblockInputRuleName.Text;
            string outputParameterNameInput = TextblockOutputParameterName.Text;
            string targetCategoryNameInput = ComboBoxInputCategory.Text;
            string trackingParameterNameInput = ComboBoxInputTargetParameter.Text;  

            // Initial check to see whether all inputs are valid; these will need to be reflected in the UI as well
            // We can probably have this check validation every time a user changes input, will need to be via event handler
            if (InputValidation.ValidateInputs(ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput))
            {
                RegexRule regexRule = RegexRuleManager.CreateRegexRule(ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput);
                ParameterServices.CreateProjectParameter(Document, outputParameterNameInput, ParameterType.Text, targetCategoryNameInput, BuiltInParameterGroup.PG_IDENTITY_DATA, true);
                ExtensibleStorageServices.SaveRegexRuleToExtensibleStorage(DocumentGuid, regexRule);
            }
            Close();
        }

        private void DeleteRegexRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            selectedRegexRuleParts.Remove((RegexRulePart)button.DataContext);
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ReorderUpButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = selectedRegexRuleParts.IndexOf(regexRulePart);

            if(index > 0)
            {
                selectedRegexRuleParts.RemoveAt(index);
                selectedRegexRuleParts.Insert(index - 1, regexRulePart);
            }            
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = selectedRegexRuleParts.IndexOf(regexRulePart);

            if (index < selectedRegexRuleParts.Count)
            {
                selectedRegexRuleParts.RemoveAt(index);
                selectedRegexRuleParts.Insert(index + 1, regexRulePart);
            }
        }
    }
}
