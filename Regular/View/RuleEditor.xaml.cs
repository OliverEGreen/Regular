using Autodesk.Revit.DB;
using Regular.ViewModel;
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

namespace Regular.View
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

            // Binding ComboBox to our RuleType enumeration
            ComboBoxRulePartInput.ItemsSource = Enum.GetValues(typeof(RuleTypes)).Cast<RuleTypes>();

            // Populating ComboBox of user-visible Revit Categories
            ListBoxCategoriesSelection.ItemsSource = RegexRule.TargetCategoryIds;
            
            // Some random parameters for now - we need the ability to look up the parameters for a particular category
            // Normally we can use a FilteredElementCollector to get these, however it's going to be tricky if we have no elements of that category
            // A workaround may involve creating a schedule and reading the schedulable parameters
            FamilyInstance door = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().OfType<FamilyInstance>().ToList().FirstOrDefault();
            List<Parameter> allProjectParameters = ParameterServices.ConvertParameterSetToList(door.Parameters).OrderBy(x => x.Definition.Name).ToList();
            ComboBoxTrackingParameterInput.ItemsSource = allProjectParameters;

            EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
            EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];

            TextBoxNameYourRuleInput.TextChanged += TextBoxNameYourRuleInput_TextChanged;
            TextBoxNameYourRuleInput.TextChanged += DisplayUserFeedback;
            TextBoxOutputParameterNameInput.TextChanged += TextBoxOutputParameterNameInput_TextChanged;
            TextBoxOutputParameterNameInput.TextChanged += DisplayUserFeedback;
            ButtonAddRulePart.Click += DisplayUserFeedback;

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
            // Testing to see whether the current rule's GUID exists
            if (!RegexRuleManager.GetDocumentRegexRuleGuids(DocumentGuid).Contains(RegexRule.Guid))
            {
                // If a new rule, a project parameter needs to be created.
                // ParameterServices.CreateProjectParameter(Document, RegexRule.OutputParameterName, ParameterType.Text, RegexRule.TargetCategoryIds, BuiltInParameterGroup.PG_IDENTITY_DATA, true);

                
                // The static RegexRule should already have inputs set by the UI forms?
                RegexRuleManager.SaveRegexRule(DocumentGuid, RegexRule);
                ExtensibleStorageServices.SaveRegexRuleToExtensibleStorage(DocumentGuid, RegexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, RegexRule.Guid);
            }
            else
            {
                // The rule already exists and is being edited. We'll generate a new temporary rule from the inputs to use as we transfer values across.
                // We don't need to create a project parameter, but we may need to update its name.
                // We need to update both the static cache and the entity saved in ExtensibleStorage.
                RegexRule newRegexRule = new RegexRule()
                {
                    Guid = RegexRule.Guid,
                    Name = RegexRule.Name,
                    TargetCategoryIds = RegexRule.TargetCategoryIds,
                    TrackingParameterName = RegexRule.TrackingParameterName,
                    OutputParameterName = RegexRule.OutputParameterName,
                    RegexString = RegexRule.RegexString,
                    RegexRuleParts = RegexRule.RegexRuleParts
                };
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
        private void ButtonExpandCategories_Click(object sender, RoutedEventArgs e)
        {
            this.ColumnCategories.Width = this.ColumnCategories.Width == new GridLength(250) ? new GridLength(0) : new GridLength(250);
        }
        private void ScrollViewerCategories_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox checkBox in ListBoxCategoriesSelection.Items) { checkBox.IsChecked = true; }
        }
        private void ButtonSelectNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox checkBox in ListBoxCategoriesSelection.Items) { checkBox.IsChecked = false; }
        }
    }
}
