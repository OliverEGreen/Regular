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
        
        private void RuleEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            RuleEditorViewModel.OutputParameterNameInputDirty = false;
            RuleEditorViewModel.RuleNameInputDirty = false;
        }

        private void ButtonEditRulePart_OnClick(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;
            IRegexRulePart regexRulePart = editButton.DataContext as IRegexRulePart;
            if (!(regexRulePart.RuleType == RuleType.FreeText)) return;
            Grid grid = WpfUtils.FindParent<Grid>(editButton);
            UIElementCollection uiElementCollection = grid.Children;
            TextBox textBox = uiElementCollection.OfType<TextBox>().FirstOrDefault();
            textBox.BorderThickness = new Thickness(1);
            textBox?.Focus();
            textBox.Select(0, textBox.Text.Length);
        }
    }
}
 