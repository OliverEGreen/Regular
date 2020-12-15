using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Views
{
    public partial class RuleEditor
    {
        public RuleEditorViewModel RuleEditorViewModel { get; set; }
        //public RuleEditorMockViewModel RuleEditorMockViewModel { get; set; }

        // The two-argument constructor is for editing an existing rule
        // We need a reference to the original rule ID, and to create a 
        // staging rule for the user to play around with until submission
        public RuleEditor(string documentGuid, RegexRule inputRule = null)
        {
            InitializeComponent();
            RuleEditorViewModel = inputRule == null ? new RuleEditorViewModel(documentGuid) : new RuleEditorViewModel(documentGuid, inputRule);
            this.DataContext = RuleEditorViewModel;
            // Lets us close the window by hitting the Escape key
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
        }
        private void ScrollViewerRuleParts_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ScrollViewerCategories_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();
        /*
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
                case RuleType.AnyLetter:
                    switch (regexRulePart.EditButtonDisplayText)
                    {
                        case "A-Z":
                            regexRulePart.EditButtonDisplayText = "a-z";
                            regexRulePart.CaseSensitiveDisplayString = "lower case";
                            break;
                        case "a-z":
                            regexRulePart.EditButtonDisplayText = "A-z";
                            regexRulePart.CaseSensitiveDisplayString = "Any Case";
                            break;
                        default:
                            regexRulePart.EditButtonDisplayText = "A-Z";
                            regexRulePart.CaseSensitiveDisplayString = "UPPER CASE";
                            break;
                    }
                    break;
                case RuleType.AnyCharacter:
                    switch (regexRulePart.EditButtonDisplayText)
                    {
                        case "AB1":
                            regexRulePart.EditButtonDisplayText = "ab1";
                            regexRulePart.CaseSensitiveDisplayString = "lower case";
                            break;
                        case "ab1":
                            regexRulePart.EditButtonDisplayText = "Ab1";
                            regexRulePart.CaseSensitiveDisplayString = "Any Case";
                            break;
                        default:
                            regexRulePart.EditButtonDisplayText = "AB1";
                            regexRulePart.CaseSensitiveDisplayString = "UPPER CASE";
                            break;
                    }
                    break;
                case RuleType.AnyDigit:
                    break;
                case RuleType.FreeText:
                    Grid grid = VisualTreeHelper.GetParent(button) as Grid;
                    UIElementCollection uiElementCollection = grid.Children;
                    TextBox textBox = null;
                    foreach (UIElement uiElement in uiElementCollection)
                    {
                        if (uiElement.GetType() == typeof(TextBox)) { textBox = uiElement as TextBox; }
                    }
                    PreviewKeyDown += (keySender, eventArgs) =>
                    {
                        if (eventArgs.Key == Key.Enter) Keyboard.ClearFocus();
                        textBox.BorderThickness = new Thickness(0);
                    };
                    textBox.IsReadOnly = false;
                    textBox.Focusable = true;
                    textBox.Focus();
                    if (textBox.Text == "Free Text")
                    {
                        textBox.Text = "Start Typing";
                        textBox.Select(0, textBox.Text.Length);
                    }
                    break;
                case RuleType.SelectionSet:
                    TaskDialog.Show("Test", "You are now editing selection set");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void ButtonExpandCategories_Click(object sender, RoutedEventArgs e)
        {
            const int categoriesPanelWidth = 250;
            const int minWindowWidth = 436;
            const int maxWindowWidth = 701;
            bool isCategoryPanelExpanded = ColumnCategories.Width == new GridLength(categoriesPanelWidth);
            
            Button button = sender as Button;
            if (!isCategoryPanelExpanded)
            {
                ColumnCategories.Width = new GridLength(categoriesPanelWidth);
                ColumnMargin.Width = new GridLength(15);
                button.Content = "Hide Categories";
                MinWidth = maxWindowWidth;
                MaxWidth = maxWindowWidth;
            }
            else
            {
                ColumnCategories.Width = new GridLength(0);
                ColumnMargin.Width = new GridLength(0);
                button.Content = "Select Categories";
                MinWidth = minWindowWidth;
                MaxWidth = minWindowWidth;
            }
        }
        private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CategoryObject observableObject in TargetCategoryObjects) { observableObject.IsChecked = true; }
        }
        private void ButtonSelectNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (CategoryObject observableObject in TargetCategoryObjects) { observableObject.IsChecked = false; }
        }
        private void ButtonTest_OnClick(object sender, RoutedEventArgs e)
        {
            if(RowExamples.Height != new GridLength(20)) RowExamples.Height = new GridLength(20);
        }
        private void ListBoxCategorySelection_SelectionChanged(object sender, RoutedEventArgs e)
        {
            void UpdateCheckedBoxesCount()
            {
                // Updates the UI Counter
                NumberCategoriesSelected = TargetCategoryObjects.Count(x => x.IsChecked);
            }
            void UpdateTrackingParameterSelectionComboBox()
            {
                ParameterObject currentParameterObject = ComboBoxTrackingParameterInput.SelectedItem as ParameterObject;
                // Updates the ComboBox to let users select parameter
                PossibleTrackingParameters = ParameterServices.GetParametersOfCategories(DocumentGuid, TargetCategoryObjects);
                if (PossibleTrackingParameters.Count > 0 && PossibleTrackingParameters.Contains(currentParameterObject)) return;
                if (NumberCategoriesSelected == 0)
                {
                    ComboBoxTrackingParameterInput.Text = "Select Categories";
                    return;
                }
                if (PossibleTrackingParameters.Count < 1)
                {
                    ComboBoxTrackingParameterInput.Text = "No Common Parameter(s)";
                    return;
                }
                if (ComboBoxTrackingParameterInput.SelectedItem == null) ComboBoxTrackingParameterInput.SelectedIndex = 0;
            }
            UpdateCheckedBoxesCount();
            UpdateTrackingParameterSelectionComboBox();
        }
        private void RegexRulePartTypeTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.IsReadOnly = false;
            textBox.BorderThickness = new Thickness(1);
        }
        private void RegexRulePartTypeTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.IsReadOnly = true;
            textBox.Focusable = false;
            textBox.BorderThickness = new Thickness(0);
        }
        private void ListBoxRuleParts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RegexRulePart regexRulePart = ((ListBox)sender).SelectedItem as RegexRulePart;
            int index = RegexRuleParts.IndexOf(regexRulePart);
            ButtonMoveRulePartUp.IsEnabled = index != 0;
            ButtonMoveRulePartDown.IsEnabled = index != regexRuleParts.Count - 1;
        }*/
    }
}
 