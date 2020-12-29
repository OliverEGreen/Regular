using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using Regular.Enums;
using Regular.Models;
using Regular.Services;
using Regular.Utilities;
using Regular.ViewModels;
using System.Collections.Generic;

namespace Regular.Views
{
    public partial class RuleEditor
    {
        public RuleEditorViewModel RuleEditorViewModel { get; set; }

        // Optional second argument constructor is for editing an existing rule
        // We need a reference to the original rule ID, and to then create a 
        // staging rule for the user to play around with until form submission / validation
        public RuleEditor(string documentGuid, RegexRule inputRule = null)
        {
            InitializeComponent();
            RuleEditorViewModel = inputRule == null ? new RuleEditorViewModel(documentGuid) : new RuleEditorViewModel(documentGuid, inputRule);
            DataContext = RuleEditorViewModel;

            // Lets us close the window by hitting the Escape key
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
            TextBoxNameYourRuleInput.Focus();
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
            // Once touched, we can display an error message if the value is invalid
            RuleEditorViewModel.RuleNameInputDirty = true;

            // Gathering other RegexRule names to ensure the user inputs a unique name
            List<RegexRule> otherRegexRules = RegexRuleCache.AllRegexRules[RuleEditorViewModel.DocumentGuid]
                    .Where(x => x != RuleEditorViewModel.StagingRule)
                    .ToList();
            string ruleNameInputFeedback = InputValidationServices.ValidateRuleName(RuleEditorViewModel.StagingRule, otherRegexRules);

            // Sets ellipse colours and feedback visibility
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
        
        private void RuleEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            RuleEditorViewModel.OutputParameterNameInputDirty = false;
            RuleEditorViewModel.RuleNameInputDirty = false;
        }

        private void ButtonControl_OnClick(object sender, RoutedEventArgs e)
        {
            // Jumping through hoops to determine if the sender RegexRulePart is 
            // of the CustomText kind
            if (!(sender is Button editButton)) return;
            if (!(editButton.DataContext is IRegexRulePart regexRulePart)) return;
            if (regexRulePart.RuleType != RuleType.CustomText) return;
            Grid grid = WpfUtils.FindParent<Grid>(editButton);
            UIElementCollection uiElementCollection = grid.Children;
            TextBox textBox = uiElementCollection
                .OfType<TextBox>()
                .FirstOrDefault(x => x.Name == "RawUserInputValueTextBox");
            if (textBox == null) return;
            
            // Sets focus to the textbox and highlights and text found in it
            textBox.Focus();
            textBox.Select(0, textBox.Text.Length);
        }

        private void RawUserInputValueTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            // Lets us close the window by hitting the Escape key
            PreviewKeyDown += (s, eventHandler) =>
            {
                if (eventHandler.Key != Key.Escape && eventHandler.Key != Key.Enter) return;
                ListBoxItem listBoxItem = WpfUtils.FindParent<ListBoxItem>(textBox);
                listBoxItem?.Focus();
            };
        }

        // If the user is allowed to submit the rule, we close the window to prevent them 
        // creating the same rule as many times as they like
        private void ButtonOk_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}
 