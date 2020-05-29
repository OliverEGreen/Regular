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
        private ObservableCollection<RegexRulePart> RegexRuleParts { get; set; }
        public RuleEditor(string documentGuid, string regexRuleGuid = null)
        {
            InitializeComponent();
            RegexRule regexRule = RegexRuleManager.GetRegexRule(documentGuid, regexRuleGuid);
                        
            // If we're editing an existing rule, it gets set to a static variable for accessibility
            if (regexRule != null) { RegexRule = regexRule; }
            Title = regexRule == null ? "Creating New Rule" : $"Editing Rule: {regexRule.RuleName}";
            InitializeRuleEditor(documentGuid, regexRule);
        }
        void InitializeRuleEditor(string documentGuid, RegexRule regexRule = null)
        {
            DocumentGuid = documentGuid;
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);

            // Clearing and Initializing the RegexRuleParts box
            RegexRuleParts = new ObservableCollection<RegexRulePart>();
            RegexRuleParts.Clear();
            RulePartsListBox.ItemsSource = RegexRuleParts;

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
                foreach(RegexRulePart regexRulePart in regexRule.RegexRuleParts) { RegexRuleParts.Add(regexRulePart); }
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
            
            // TODO: Change these paradigms to those outlined in the RegexAssemblyService
            // In fact, move this code to the RegexAssemblyService classs
            RegexRulePart newRegexRulePart = new RegexRulePart("'.' (Full Stop)", (RuleTypes)ComboBoxInputRulePartType.SelectedItem, false, false);
            
            if (selectedRuleType == RuleTypes.AnyLetter | selectedRuleType == RuleTypes.AnyCharacter | selectedRuleType == RuleTypes.AnyNumber | selectedRuleType == RuleTypes.Anything)
            {
                newRegexRulePart.DisplayString = "Any Digit";
                newRegexRulePart.RawUserInputValue = "";
                RegexRuleParts.Add(newRegexRulePart);
            }
            else if(selectedRuleType == RuleTypes.Dot)
            {
                newRegexRulePart.DisplayString = "Full Stop";
                newRegexRulePart.RawUserInputValue = "";
                RegexRuleParts.Add(newRegexRulePart);
            }
            else if (selectedRuleType == RuleTypes.Hyphen)
            {
                newRegexRulePart.DisplayString = "Hyphen";
                newRegexRulePart.RawUserInputValue = "";
                RegexRuleParts.Add(newRegexRulePart);
            }
            else if (selectedRuleType == RuleTypes.Underscore)
            {
                newRegexRulePart.DisplayString = "Underscore";
                newRegexRulePart.RawUserInputValue = "";
                RegexRuleParts.Add(newRegexRulePart);
            }
            else
            {
                newRegexRulePart.DisplayString = "Specific Character";
                newRegexRulePart.RawUserInputValue = " ";
                RegexRuleParts.Add(newRegexRulePart);
            }
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            string ruleNameInput = TextblockInputRuleName.Text;
            string outputParameterNameInput = TextblockOutputParameterName.Text;
            string targetCategoryNameInput = ComboBoxInputCategory.Text;
            string trackingParameterNameInput = ComboBoxInputTargetParameter.Text;
            string regexStringInput = RegexAssembly.AssembleRegexString(RegexRuleParts);
            
            // Initial check to see whether all inputs are valid; these will need to be reflected in the UI as well
            // We can probably have this check validation every time a user changes input, will need to be via event handler
            if (!InputValidationServices.ValidateInputs(ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, RegexRuleParts)) return;

            if (RegexRule == null)
            {
                // Takes all information from the form and builds a new RegexRule object. Needs to be saved in cache and in local storage.
                // If a new rule, a project parameter needs to be created.
                RegexRule regexRule = RegexRuleManager.AddRegexRule(DocumentGuid, ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, RegexRuleParts);
                ParameterServices.CreateProjectParameter(Document, outputParameterNameInput, ParameterType.Text, targetCategoryNameInput, BuiltInParameterGroup.PG_IDENTITY_DATA, true);
                ExtensibleStorageServices.AddRegexRuleToExtensibleStorage(DocumentGuid, regexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, regexRule.Guid);
            }
            else
            {
                // The rule already exists and is being edited. We'll generate a new temporary rule from the inputs to use as we transfer values across.
                // We don't need to create a project parameter, but we may need to update its name.
                // We need to update both the static cache and the entity saved in ExtensibleStorage.
                RegexRule newRegexRule = new RegexRule(ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, RegexRuleParts);
                RegexRuleManager.UpdateRegexRule(DocumentGuid, RegexRule.Guid, newRegexRule);
                ExtensibleStorageServices.UpdateRegexRuleInExtensibleStorage(DocumentGuid, RegexRule.Guid, newRegexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, RegexRule.Guid);
            }
            Close();
        }
        private void DeleteRegexRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRuleParts.Remove((RegexRulePart)button.DataContext);
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ReorderUpButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = RegexRuleParts.IndexOf(regexRulePart);

            if(index > 0)
            {
                RegexRuleParts.RemoveAt(index);
                RegexRuleParts.Insert(index - 1, regexRulePart);
            }            
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = RegexRuleParts.IndexOf(regexRulePart);

            if (index < RegexRuleParts.Count)
            {
                RegexRuleParts.RemoveAt(index);
                RegexRuleParts.Insert(index + 1, regexRulePart);
            }
        }
    }
}
