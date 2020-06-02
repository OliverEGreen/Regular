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
using System.Windows.Media;

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
            Title = regexRule == null ? "Creating New Rule" : $"Editing Rule: {regexRule.Name}";
            InitializeRuleEditor(documentGuid, regexRule);
        }
        void InitializeRuleEditor(string documentGuid, RegexRule regexRule = null)
        {
            DocumentGuid = documentGuid;
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);

            // Clearing and Initializing the RegexRuleParts box
            RegexRuleParts = new ObservableCollection<RegexRulePart>();
            RegexRuleParts.Clear();
            ListBoxRuleParts.ItemsSource = RegexRuleParts;

            // Binding ComboBox to our RuleType enumeration
            ComboBoxRulePartInput.ItemsSource = Enum.GetValues(typeof(RuleTypes)).Cast<RuleTypes>();

            // Populating ComboBox of user-visible Revit Categories
            List<string> userVisibleCategoryNames = CategoryServices.GetListFromCategorySet(Document.Settings.Categories).Where(x => x.AllowsBoundParameters).Select(i => i.Name).OrderBy(i => i).ToList();
            ComboBoxCategoryInput.Items.Clear();
            ComboBoxCategoryInput.ItemsSource = userVisibleCategoryNames;
            
            if (regexRule != null)
            {
                TextBoxNameYourRuleInput.Text = regexRule.Name;
                TextBoxOutputParameterNameInput.Text = regexRule.OutputParameterName;
                ComboBoxTrackingParameterInput.SelectedItem = regexRule.TrackingParameterName;
                ComboBoxCategoryInput.SelectedItem = regexRule.TargetCategoryNames;
                foreach(RegexRulePart regexRulePart in regexRule.RegexRuleParts) { RegexRuleParts.Add(regexRulePart); }
            }

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
            RegexRuleParts.Add(RulePartServices.CreateRegexRulePart((RuleTypes)ComboBoxRulePartInput.SelectedItem));
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            string ruleNameInput = TextBoxNameYourRuleInput.Text;
            string outputParameterNameInput = TextBoxOutputParameterNameInput.Text;
            string targetCategoryNameInput = ComboBoxCategoryInput.Text;
            string trackingParameterNameInput = ComboBoxTrackingParameterInput.Text;
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
        private void DisplayUserFeedback(object sender, RoutedEventArgs e)
        {
            string userFeedback = InputValidationServices.ReturnUserFeedback(TextBoxNameYourRuleInput.Text, TextBoxOutputParameterNameInput.Text, RegexRuleParts);
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

        }
    }
}
