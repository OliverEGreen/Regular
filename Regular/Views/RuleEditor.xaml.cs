using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using Regular.Models;
using Regular.Services;
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
            DataContext = RuleEditorViewModel;
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
        
        private void TextBoxNameYourRuleInput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RuleEditorViewModel.RuleNameInputDirty = true;
            string ruleNameInputFeedback = InputValidationServices.ValidateRuleName(RuleEditorViewModel.StagingRule.RuleName);
            TextBox textBox = (TextBox)sender;

            if (textBox.Text.Length < 1)
            {
                if (RuleEditorViewModel.RuleNameInputDirty)
                {
                    EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorRed"];
                    RuleEditorViewModel.UserFeedbackText = ruleNameInputFeedback;
                    RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Visible;
                }
                else
                {
                    EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
                    RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Hidden;
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(ruleNameInputFeedback))
            {
                EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGreen"];
                RuleEditorViewModel.UserFeedbackText = "";
                RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Hidden;
            }
            else
            {
                EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorRed"];
                RuleEditorViewModel.UserFeedbackText = ruleNameInputFeedback;
                RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Visible;
            }
        }

        private void TextBoxOutputParameterNameInput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RuleEditorViewModel.OutputParameterNameInputDirty = true;
            string outputParameterNameFeedback = InputValidationServices.ValidateOutputParameterName(RuleEditorViewModel.StagingRule.OutputParameterObject.ParameterObjectName);
            TextBox textBox = (TextBox)sender;

            if (textBox.Text.Length < 1)
            {
                if (RuleEditorViewModel.OutputParameterNameInputDirty)
                {
                    EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorRed"];
                    RuleEditorViewModel.UserFeedbackText = outputParameterNameFeedback;
                    RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Visible;
                }
                else
                {
                    EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
                    RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Hidden;
                }
                return;
            }
            
            if (string.IsNullOrWhiteSpace(outputParameterNameFeedback))
            {
                EllipseOutputParameterNameInput.Fill = (SolidColorBrush) this.Resources["EllipseColorGreen"];
                RuleEditorViewModel.UserFeedbackText = "";
                RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Hidden;
            }
            else
            {
                EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorRed"];
                RuleEditorViewModel.UserFeedbackText = outputParameterNameFeedback;
                RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Visible;
            }
        }

        /*
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
                PossibleTrackingParameterObjects = ParameterServices.GetParametersOfCategories(DocumentGuid, TargetCategoryObjects);
                if (PossibleTrackingParameterObjects.Count > 0 && PossibleTrackingParameterObjects.Contains(currentParameterObject)) return;
                if (NumberCategoriesSelected == 0)
                {
                    ComboBoxTrackingParameterInput.Text = "Select Categories";
                    return;
                }
                if (PossibleTrackingParameterObjects.Count < 1)
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
        private void RuleEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            RuleEditorViewModel.OutputParameterNameInputDirty = false;
            RuleEditorViewModel.RuleNameInputDirty = false;
        }
    }
}
 