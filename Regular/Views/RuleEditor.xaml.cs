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
        private static Document Document { get; set; }
        private static string DocumentGuid { get; set; }
        private RegexRule RegexRule { get; set; }
        private ObservableCollection<RegexRulePart> SelectedRegexRuleParts { get; set; }

        public RuleEditor(string documentGuid, string regexRuleGuid = null)
        {
            InitializeComponent();
            RegexRule regexRule = RegexRuleManager.GetRegexRule(documentGuid, regexRuleGuid);
                        
            // If we're editing an existing rule, setting it as a static variable for accessibility
            if (regexRule != null) { RegexRule = regexRule; }
            Title = regexRule == null ? "Creating New Rule" : $"Editing Rule: {regexRule.RuleName}";
            InitializeRuleEditor(documentGuid, regexRule);
        }
        void InitializeRuleEditor(string documentGuid, RegexRule regexRule = null)
        {
            DocumentGuid = documentGuid;
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);

            // Clearing and Initializing the RegexRuleParts box
            SelectedRegexRuleParts = new ObservableCollection<RegexRulePart>();
            SelectedRegexRuleParts.Clear();
            RulePartsListBox.ItemsSource = SelectedRegexRuleParts;

            // Binding ComboBox to our RuleType enumeration
            ComboBoxInputRulePartType.ItemsSource = Enum.GetValues(typeof(RuleTypes)).Cast<RuleTypes>();

            // Populating ComboBox of user-visible Revit Categories
            List<string> userVisibleCategoryNames = CategoryServices.GetListFromCategorySet(Document.Settings.Categories).Where(x => x.AllowsBoundParameters).Select(i => i.Name).OrderBy(i => i).ToList();
            ComboBoxInputCategory.Items.Clear();
            ComboBoxInputCategory.ItemsSource = userVisibleCategoryNames;
            
            if (regexRule != null)
            {
                TextblockInputRuleName.Text = regexRule.RuleName;
                TextblockOutputParameterName.Text = regexRule.OutputParameterName;
                ComboBoxInputTargetParameter.SelectedItem = regexRule.TrackingParameterName;
                ComboBoxInputCategory.SelectedItem = regexRule.TargetCategoryName;
                foreach(RegexRulePart regexRulePart in regexRule.RegexRuleParts) { SelectedRegexRuleParts.Add(regexRulePart); }
            }
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
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
            string regexStringInput = RegexAssembly.AssembleRegexString(SelectedRegexRuleParts);
            
            // Initial check to see whether all inputs are valid; these will need to be reflected in the UI as well
            // We can probably have this check validation every time a user changes input, will need to be via event handler
            if (!InputValidation.ValidateInputs(ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, SelectedRegexRuleParts)) return;

            if (RegexRule == null)
            {
                // Takes all information from the form and builds a new RegexRule object. Needs to be saved in cache and in local storage.
                // If a new rule, a project parameter needs to be created.
                RegexRule regexRule = RegexRuleManager.AddRegexRule(DocumentGuid, ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, SelectedRegexRuleParts);
                ParameterServices.CreateProjectParameter(Document, outputParameterNameInput, ParameterType.Text, targetCategoryNameInput, BuiltInParameterGroup.PG_IDENTITY_DATA, true);
                ExtensibleStorageServices.AddRegexRuleToExtensibleStorage(DocumentGuid, regexRule);
            }
            else
            {
                // The rule already exists and is being edited. We'll generate a new temporary rule from the inputs to use as we transfer values across.
                // We don't need to create a project parameter, but we may need to update its name.
                // We need to update both the static cache and the entity saved in ExtensibleStorage.
                RegexRule newRegexRule = new RegexRule(ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, SelectedRegexRuleParts);
                RegexRuleManager.UpdateRegexRule(DocumentGuid, RegexRule.Guid, newRegexRule);
                ExtensibleStorageServices.UpdateRegexRuleInExtensibleStorage(DocumentGuid, RegexRule.Guid, newRegexRule);
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
