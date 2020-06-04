using Autodesk.Revit.DB;
using Regular.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System;
using System.Windows.Controls;
using Regular.Services;
using System.Windows.Media;
using Autodesk.Revit.UI;
using TextBox = System.Windows.Controls.TextBox;

namespace Regular.Views
{
    public partial class RuleEditor : Window
    {
        private static Document Document { get; set; }
        private static string DocumentGuid { get; set; }
        private RegexRule RegexRule { get; set; }
        public RuleEditor(string documentGuid, RegexRule regexRule)
        {
            InitializeComponent();
            RegexRule = regexRule;
            this.DataContext = RegexRule;

            // If we're editing an existing rule, it gets set to a static variable for accessibility
            Title = RegexRuleManager.GetDocumentRegexRules(documentGuid).Contains(regexRule) ? $"Editing Rule: {regexRule.Name}" : "Creating New Rule";
            InitializeRuleEditor(documentGuid, regexRule);
        }
        void InitializeRuleEditor(string documentGuid, RegexRule regexRule)
        {
            DocumentGuid = documentGuid;
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);

            ListBoxRuleParts.ItemsSource = regexRule.RegexRuleParts;

            // Binding ComboBox to our RuleType enumeration
            ComboBoxRulePartInput.ItemsSource = Enum.GetValues(typeof(RuleTypes)).Cast<RuleTypes>();

            // Populating ComboBox of user-visible Revit Categories
            List<string> userVisibleCategoryNames = CategoryServices.GetListFromCategorySet(Document.Settings.Categories).Where(x => x.AllowsBoundParameters).Select(i => i.Name).OrderBy(i => i).ToList();
            ComboBoxCategoryInput.Items.Clear();
            ComboBoxCategoryInput.ItemsSource = userVisibleCategoryNames;
            
            EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
            EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];

            TextBoxNameYourRuleInput.TextChanged += TextBoxNameYourRuleInput_TextChanged;
            TextBoxNameYourRuleInput.TextChanged += DisplayUserFeedback;
            TextBoxOutputParameterNameInput.TextChanged += TextBoxOutputParameterNameInput_TextChanged;
            TextBoxOutputParameterNameInput.TextChanged += DisplayUserFeedback;

            TextBoxUserFeedback.Visibility = System.Windows.Visibility.Hidden;
        }
        private void ScrollViewerRuleParts_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ButtonAddRulePart_Click(object sender, RoutedEventArgs e)
        {
            RegexRule.RegexRuleParts.Add(RulePartServices.CreateRegexRulePart((RuleTypes)ComboBoxRulePartInput.SelectedItem));
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            // Initial check to see whether all inputs are valid; these will need to be reflected in the UI as well
            // We can probably have this check validation every time a user changes input, will need to be via event handler
            
            //if (!InputValidationServices.ValidateInputs(ruleNameInput, targetCategoryNameInput, trackingParameterNameInput, outputParameterNameInput, regexStringInput, RegexRuleParts)) return;

            if (!RegexRuleManager.GetDocumentRegexRules(DocumentGuid).Contains(RegexRule))
            {
                // If a new rule, a project parameter needs to be created.
                ParameterServices.CreateProjectParameter(Document, RegexRule.OutputParameterName, ParameterType.Text, RegexRule.TargetCategoryNames, BuiltInParameterGroup.PG_IDENTITY_DATA, true);
                
                RegexRule regexRule = RegexRuleManager.SaveRegexRule(DocumentGuid, RegexRule.Name, RegexRule.TargetCategoryNames, RegexRule.TrackingParameterName, RegexRule.OutputParameterName, RegexRule.RegexString, RegexRule.RegexRuleParts);
                ExtensibleStorageServices.SaveRegexRuleToExtensibleStorage(DocumentGuid, regexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, regexRule.Guid);
            }
            else
            {
                // The rule already exists and is being edited. We'll generate a new temporary rule from the inputs to use as we transfer values across.
                // We don't need to create a project parameter, but we may need to update its name.
                // We need to update both the static cache and the entity saved in ExtensibleStorage.
                RegexRule newRegexRule = new RegexRule(RegexRule.Name, RegexRule.TargetCategoryNames, RegexRule.TrackingParameterName, RegexRule.OutputParameterName, RegexRule.RegexString, RegexRule.RegexRuleParts);
                RegexRuleManager.UpdateRegexRule(DocumentGuid, RegexRule.Guid, newRegexRule);
                ExtensibleStorageServices.UpdateRegexRuleInExtensibleStorage(DocumentGuid, RegexRule.Guid, newRegexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, RegexRule.Guid);
            }
            Close();
        }
        private void DeleteRegexRulePartButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRule.RegexRuleParts.Remove((RegexRulePart)button.DataContext);
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ReorderUpButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = RegexRule.RegexRuleParts.IndexOf(regexRulePart);

            if(index > 0)
            {
                RegexRule.RegexRuleParts.RemoveAt(index);
                RegexRule.RegexRuleParts.Insert(index - 1, regexRulePart);
            }            
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = RegexRule.RegexRuleParts.IndexOf(regexRulePart);

            if (index < RegexRule.RegexRuleParts.Count)
            {
                RegexRule.RegexRuleParts.RemoveAt(index);
                RegexRule.RegexRuleParts.Insert(index + 1, regexRulePart);
            }
        }
        private void DisplayUserFeedback(object sender, RoutedEventArgs e)
        {
            string userFeedback = InputValidationServices.ReturnUserFeedback(TextBoxNameYourRuleInput.Text, TextBoxOutputParameterNameInput.Text, RegexRule.RegexRuleParts);
            if (userFeedback == null)
            {
                TextBoxUserFeedback.Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            TextBoxUserFeedback.Visibility = System.Windows.Visibility.Visible;
            TextBoxUserFeedback.Text = userFeedback;
        }
        private void TextBoxOutputParameterNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length < 1)
            {
                EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
                return;
            }
            bool ruleNameInputValid = InputValidationServices.ValidateOutputParameterName(textBox.Text);
            EllipseOutputParameterNameInput.Fill = ruleNameInputValid ? (SolidColorBrush)this.Resources["EllipseColorGreen"] : (SolidColorBrush)this.Resources["EllipseColorRed"];
        }
        private void TextBoxNameYourRuleInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length < 1)
            {
                EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
                return;
            }
            bool ruleNameInputValid = InputValidationServices.ValidateRuleName(textBox.Text);
            EllipseNameYourRuleInput.Fill = ruleNameInputValid ? (SolidColorBrush)this.Resources["EllipseColorGreen"] : (SolidColorBrush)this.Resources["EllipseColorRed"];
        }
        private void ButtonEditRulePart_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = button.DataContext as RegexRulePart;
            switch (regexRulePart.RuleType)
            {
                case RuleTypes.AnyLetter:
                    if (regexRulePart.CaseSensitiveDisplayString == "UPPER CASE") regexRulePart.CaseSensitiveDisplayString = "lower case";
                    else if (regexRulePart.CaseSensitiveDisplayString == "lower case") regexRulePart.CaseSensitiveDisplayString = "Any Case";
                    else { regexRulePart.CaseSensitiveDisplayString = "UPPER CASE"; }
                    break;
                case RuleTypes.AnyDigit:
                    break;
                case RuleTypes.FreeText:
                    TaskDialog.Show("Test", "You are now editing free text");
                    regexRulePart.RuleTypeDisplayText = "My example text";
                    break;
                case RuleTypes.SelectionSet:
                    TaskDialog.Show("Test", "You are now editing selection set");
                    break;
            }
        }
    }
}
