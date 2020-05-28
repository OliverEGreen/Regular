using Autodesk.Revit.DB;
using Regular.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System;
using System.Windows.Controls;
using Regular.Services;

namespace Regular.Views
{
    public partial class RuleEditor : Window
    {
        public static Document Document { get; set; }
        public static string DocumentGuid { get; set; }
        public ObservableCollection<RegexRulePart> SelectedRegexRuleParts { get; set; }

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

        // Constructor for creating a new RegexRule
        public RuleEditor(string documentGuid)
        {
            InitializeComponent();
            Title = "Creating New Rule";
            
            DocumentGuid = documentGuid;
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            
            // Clearing and binding the RuleParts ListBox
            SelectedRegexRuleParts = new ObservableCollection<RegexRulePart>();
            SelectedRegexRuleParts.Clear();
            RulePartsListBox.ItemsSource = SelectedRegexRuleParts;

            // Populating ComboBox of user-visible Revit Categories
            List<string> userVisibleCategoryNames = CategoryServices.GetListFromCategorySet(Document.Settings.Categories).Where(x => x.AllowsBoundParameters).Select(i => i.Name).OrderBy(i => i).ToList();
            ComboBoxInputCategory.Items.Clear();
            ComboBoxInputCategory.ItemsSource = userVisibleCategoryNames;

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
            string selectedCategoryName = ComboBoxInputCategory.SelectedValue.ToString();
            // We need the ability to fetch parameters that are bound to the selected category. WIP
        }

        private void AddRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            RuleTypes selectedRuleType = (RuleTypes)ComboBoxInputRulePartType.SelectedItem;

            RegexRulePart newRegexRulePart = new RegexRulePart("'.' (Full Stop)", (RuleTypes)ComboBoxInputRulePartType.SelectedItem, false, false);
            
            if (selectedRuleType == RuleTypes.AnyLetter | selectedRuleType == RuleTypes.AnyCharacter | selectedRuleType == RuleTypes.AnyNumber | selectedRuleType == RuleTypes.Anything)
            {
                newRegexRulePart.DisplayString = "Any Digit";
                newRegexRulePart.RawUserInputValue = "";
                SelectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if(selectedRuleType == RuleTypes.Dot)
            {
                newRegexRulePart.DisplayString = "Full Stop";
                newRegexRulePart.RawUserInputValue = "";
                SelectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if (selectedRuleType == RuleTypes.Hyphen)
            {
                newRegexRulePart.DisplayString = "Hyphen";
                newRegexRulePart.RawUserInputValue = "";
                SelectedRegexRuleParts.Add(newRegexRulePart);
            }
            else if (selectedRuleType == RuleTypes.Underscore)
            {
                newRegexRulePart.DisplayString = "Underscore";
                newRegexRulePart.RawUserInputValue = "";
                SelectedRegexRuleParts.Add(newRegexRulePart);
            }
            else
            {
                newRegexRulePart.DisplayString = "Specific Character";
                newRegexRulePart.RawUserInputValue = " ";
                SelectedRegexRuleParts.Add(newRegexRulePart);
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            string ruleNameInput = TextblockInputRuleName.Text;
            string outputParameterNameInput = TextblockOutputParameterName.Text;
            string targetCategoryNameInput = ComboBoxInputCategory.Text;
            string trackingParameterNameInput = ComboBoxInputTargetParameter.Text;
            string regexStringInput = "";
            foreach (RegexRulePart regexRulePart in SelectedRegexRuleParts) { regexStringInput += GetRegexPartFromRuleType(regexRulePart); }

            // Initial check to see whether all inputs are valid; these will need to be reflected in the UI as well
            // We can probably have this check validation every time a user changes input, will need to be via event handler
            if (InputValidation.ValidateInputs(ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, SelectedRegexRuleParts))
            {
                // Takes all information from the form and builds our RegexRule object. Needs to be saved in cache and in local storage.
                // If a new rule, a project parameter needs to be created.
                RegexRule regexRule = RegexRuleManager.AddRegexRule(DocumentGuid, ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, SelectedRegexRuleParts);
                ParameterServices.CreateProjectParameter(Document, outputParameterNameInput, ParameterType.Text, targetCategoryNameInput, BuiltInParameterGroup.PG_IDENTITY_DATA, true);
                ExtensibleStorageServices.SaveRegexRuleToExtensibleStorage(DocumentGuid, regexRule);
            }
            Close();
        }

        private void DeleteRegexRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SelectedRegexRuleParts.Remove((RegexRulePart)button.DataContext);
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ReorderUpButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = SelectedRegexRuleParts.IndexOf(regexRulePart);

            if(index > 0)
            {
                SelectedRegexRuleParts.RemoveAt(index);
                SelectedRegexRuleParts.Insert(index - 1, regexRulePart);
            }            
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = SelectedRegexRuleParts.IndexOf(regexRulePart);

            if (index < SelectedRegexRuleParts.Count)
            {
                SelectedRegexRuleParts.RemoveAt(index);
                SelectedRegexRuleParts.Insert(index + 1, regexRulePart);
            }
        }
    }
}
