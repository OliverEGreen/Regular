using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Regular.Enums;
using Regular.ViewModel;
using Regular.Model;
using Regular.Services;
using Button = System.Windows.Controls.Button;
using TextBox = System.Windows.Controls.TextBox;

namespace Regular.View
{
    public partial class RuleEditor: INotifyPropertyChanged
    {
        private static Document Document { get; set; }
        private static string DocumentGuid { get; set; }
        private int numberCategoriesSelected;
        public int NumberCategoriesSelected
        {
            get => numberCategoriesSelected;
            set
            {
                numberCategoriesSelected = ListBoxCategoriesSelection.SelectedItems.Count;
                OnPropertyChanged("NumberCategoriesSelected");
            }
        }
        private RegexRule RegexRule { get; }
        public RuleEditor(string documentGuid, RegexRule regexRule)
        {
            InitializeComponent();
            // Gives us the ability to close the window with the Esc kay
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
            RegexRule = regexRule;
            this.DataContext = RegexRule;

            // If we're editing an existing rule, it gets set to a static variable for accessibility
            Title = RegexRule.GetDocumentRegexRules(documentGuid).Contains(regexRule) ? $"Editing Rule: {regexRule.Name}" : "Creating New Rule";
            TextBoxOutputParameterNameInput.IsEnabled = !RegexRules.AllRegexRules[documentGuid].Contains(regexRule);
            InitializeRuleEditor(documentGuid);
        }
        private void InitializeRuleEditor(string documentGuid)
        {
            DocumentGuid = documentGuid;
            Document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            
            // Binding ComboBox to our RuleType enumeration
            ComboBoxRulePartInput.ItemsSource = Enum.GetValues(typeof(RuleTypes)).Cast<RuleTypes>();
            
            // Populating ComboBox of user-visible Revit Categories
            ListBoxCategoriesSelection.ItemsSource = RegexRule.TargetCategoryIds;

            TextBoxOutputParameterNameInput.MaxLength = InputValidationServices.MaxInputLength;
            TextBoxNameYourRuleInput.MaxLength = InputValidationServices.MaxInputLength;

            // Some random parameters for now - we need the ability to look up the parameters for a particular category
            // Normally we can use a FilteredElementCollector to get these, however it's going to be tricky if we have no elements of that category
            // A workaround may involve creating a schedule and reading the schedulable parameters
            FamilyInstance door = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().OfType<FamilyInstance>().ToList().FirstOrDefault();
            if (door != null)
            {
                List<Parameter> allProjectParameters = ParameterServices.ConvertParameterSetToList(door.Parameters).OrderBy(x => x.Definition.Name).ToList();
                ComboBoxTrackingParameterInput.ItemsSource = allProjectParameters;
            }

            EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
            EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];

            TextBoxNameYourRuleInput.TextChanged += TextBoxNameYourRuleInput_TextChanged;
            TextBoxNameYourRuleInput.TextChanged += DisplayUserFeedback;
            TextBoxOutputParameterNameInput.TextChanged += TextBoxOutputParameterNameInput_TextChanged;
            TextBoxOutputParameterNameInput.TextChanged += DisplayUserFeedback;

            RegexRule.RegexRuleParts.CollectionChanged += UpdateExampleText;
            RegexRule.RegexRuleParts.CollectionChanged += DisplayUserFeedback;
            RegexRule.TargetCategoryIds.CollectionChanged += DisplayUserFeedback;
            ButtonTest.Click += UpdateExampleText;
            
            TextBoxUserFeedback.Visibility = System.Windows.Visibility.Hidden;
            ButtonTest.IsEnabled = RegexRule.RegexRuleParts.Count > 0;
            TextBoxNameYourRuleInput.Focus();
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
            TextBlockExample.Text = RegexAssembly.GenerateRandomExample(RegexRule.RegexRuleParts);
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            // Testing to see whether the current rule's GUID exists
            if (!RegexRule.GetDocumentRegexRuleGuids(DocumentGuid).Contains(RegexRule.Guid))
            {
                RegexRule.Save(DocumentGuid, RegexRule);
                ExtensibleStorageServices.SaveRegexRuleToExtensibleStorage(DocumentGuid, RegexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, RegexRule.Guid);
                // If a new rule, a project parameter needs to be created.
                // ParameterServices.CreateProjectParameter(Document, RegexRule.OutputParameterName, ParameterType.Text, RegexRule.TargetCategoryIds, BuiltInParameterGroup.PG_IDENTITY_DATA, true);
            }
            else
            {
                // The rule already exists and is being edited. We'll generate a new temporary rule from the inputs to use as we transfer values across.
                RegexRule regexRule = RegexRule.Create(DocumentGuid, RegexRule.Guid);
                {
                    regexRule.Name = RegexRule.Name;
                    regexRule.TargetCategoryIds = RegexRule.TargetCategoryIds;
                    regexRule.TrackingParameterName = RegexRule.TrackingParameterName;
                    regexRule.OutputParameterName = RegexRule.OutputParameterName;
                    regexRule.RegexString = RegexRule.RegexString;
                    regexRule.RegexRuleParts = RegexRule.RegexRuleParts;
                };
                // We update both the static cache and the entity saved in ExtensibleStorage.
                RegexRule.Update(DocumentGuid, RegexRule.Guid, regexRule);
                ExtensibleStorageServices.UpdateRegexRuleInExtensibleStorage(DocumentGuid, RegexRule.Guid, regexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, RegexRule.Guid);
            }
            Close();
        }
        private void ButtonDeleteRegexRulePart_OnClick(object sender, RoutedEventArgs e)
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

            if (index > 0)
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
            if (string.IsNullOrEmpty(userFeedback))
            {
                TextBoxUserFeedback.Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            TextBoxUserFeedback.Visibility = System.Windows.Visibility.Visible;
            TextBoxUserFeedback.Text = userFeedback;
        }
        private void DisplayUserFeedback(object sender, NotifyCollectionChangedEventArgs e)
        {
            string userFeedback = InputValidationServices.ReturnUserFeedback(TextBoxNameYourRuleInput.Text, TextBoxOutputParameterNameInput.Text, RegexRule.RegexRuleParts);
            if (string.IsNullOrEmpty(userFeedback))
            {
                TextBoxUserFeedback.Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            TextBoxUserFeedback.Visibility = System.Windows.Visibility.Visible;
            TextBoxUserFeedback.Text = userFeedback;
        }
        private void UpdateExampleText(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (RegexRule.RegexRuleParts.Count <= 0)
            {
                ButtonTest.IsEnabled = false;
                return;
            }
            ButtonTest.IsEnabled = true;
            TextBlockExample.Text = RegexAssembly.GenerateRandomExample(RegexRule.RegexRuleParts);
        }
        private void UpdateExampleText(object sender, RoutedEventArgs e)
        {
            TextBlockExample.Text = RegexAssembly.GenerateRandomExample(RegexRule.RegexRuleParts);
        }
        private void TextBoxOutputParameterNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length < 1)
            {
                EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
                return;
            }
            bool ruleNameInputValid = string.IsNullOrEmpty(InputValidationServices.ValidateOutputParameterName(textBox.Text));
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
            EllipseNameYourRuleInput.Fill = string.IsNullOrEmpty(InputValidationServices.ValidateRuleName(textBox.Text)) ? (SolidColorBrush)this.Resources["EllipseColorGreen"] : (SolidColorBrush)this.Resources["EllipseColorRed"];
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
            Button button = sender as Button;
            button.Content = this.ColumnCategories.Width == new GridLength(250) ? "Select Categories" : "Hide Categories";
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
            foreach (ObservableObject observableObject in RegexRule.TargetCategoryIds) { observableObject.IsChecked = true; }
        }
        private void ButtonSelectNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (ObservableObject observableObject in RegexRule.TargetCategoryIds) { observableObject.IsChecked = false; }
        }
        private void ButtonTest_OnClick(object sender, RoutedEventArgs e)
        {
            if(this.RowExamples.Height != new GridLength(20)) this.RowExamples.Height = new GridLength(20);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
 