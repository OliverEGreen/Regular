using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Controls;
using Autodesk.Revit.UI;
using Regular.Enums;
using Regular.ViewModel;
using Regular.Services;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using TextBox = System.Windows.Controls.TextBox;
using Visibility = System.Windows.Visibility;

namespace Regular.View
{
    public partial class RuleEditor: INotifyPropertyChanged
    {
        private static string DocumentGuid { get; set; }
        private string ruleName;
        private ObservableCollection<CategoryObject> targetCategoryIds;
        private ObservableCollection<ParameterObject> possibleTrackingParameters;
        private ObservableCollection<RegexRulePart> regexRuleParts;
        private ParameterObject trackingParameterObject;
        private ParameterObject outputParameterObject;
        private MatchType matchType;
        private int numberCategoriesSelected;
        private string userFeedbackText;
        private bool IsFrozen { get; set; }
        private bool EditingExistingRule { get; set; }
        private string ExistingRuleGuid { get; set; }
        
        public string RuleName
        {
            get => ruleName;
            set
            {
                ruleName = value;
                NotifyPropertyChanged("RuleName");
            }
        }
        public ObservableCollection<CategoryObject> TargetCategoryIds
        {
            get => targetCategoryIds;
            set
            {
                targetCategoryIds = value;
                NotifyPropertyChanged("TargetCategoryIds");
            }
        }
        public ObservableCollection<ParameterObject> PossibleTrackingParameters
        {
            get => possibleTrackingParameters;
            set
            {
                possibleTrackingParameters = value;
                NotifyPropertyChanged("PossibleTrackingParameters");
            }
        }
        public ParameterObject TrackingParameterObject
        {
            get => trackingParameterObject;
            set
            {
                trackingParameterObject = value;
                NotifyPropertyChanged("TrackingParameterObject");
            }
        }
        public ParameterObject OutputParameterObject
        {
            get => outputParameterObject;
            set
            {
                outputParameterObject = value;
                NotifyPropertyChanged("OutputParameterObject");
            }
        }
        public ObservableCollection<RegexRulePart> RegexRuleParts
        {
            get => regexRuleParts;
            set
            {
                regexRuleParts = value;
                NotifyPropertyChanged("RegexRuleParts");
            }
        }
        public MatchType MatchType
        {
            get => matchType;
            set
            {
                matchType = value;
                NotifyPropertyChanged("MatchType");
            }
        }
        public int NumberCategoriesSelected
        {
            get => numberCategoriesSelected;
            set
            {
                numberCategoriesSelected = value;
                NotifyPropertyChanged("NumberCategoriesSelected");
            }
        }
        public string UserFeedbackText
        {
            get => userFeedbackText;
            set
            {
                userFeedbackText = value;
                NotifyPropertyChanged("UserFeedbackText");
            }
        }
       
        public RuleEditor(string documentGuid, RegexRule inputRegexRule)
        {
            InitializeComponent();
            DataContext = this;
            InitializeRuleEditor(documentGuid, inputRegexRule);
        }
        private void InitializeRuleEditor(string documentGuid, RegexRule inputRegexRule)
        {
            DocumentGuid = documentGuid;
            RuleName = inputRegexRule.RuleName;
            TargetCategoryIds = inputRegexRule.TargetCategoryIds;
            RegexRuleParts = inputRegexRule.RegexRuleParts;
            TrackingParameterObject = inputRegexRule.TrackingParameterObject;
            OutputParameterObject = inputRegexRule.OutputParameterObject;
            MatchType = inputRegexRule.MatchType;
            NumberCategoriesSelected = targetCategoryIds.Count(x => x.IsChecked);
            IsFrozen = inputRegexRule.IsFrozen;

            Title = EditingExistingRule ? $"Editing Rule: {RuleName}" : "Creating New Rule";
            TextBoxOutputParameterNameInput.IsEnabled = ! EditingExistingRule;

            EditingExistingRule = RegexRule.GetDocumentRegexRuleGuids(documentGuid).Contains(inputRegexRule.RuleGuid);
            if (EditingExistingRule)
            {
                ExistingRuleGuid = inputRegexRule.RuleGuid;
                PossibleTrackingParameters = ParameterServices.GetParametersOfCategories(DocumentGuid, TargetCategoryIds);
                ComboBoxTrackingParameterInput.SelectedItem = PossibleTrackingParameters
                    .FirstOrDefault(x =>
                    x.ParameterObjectId == TrackingParameterObject.ParameterObjectId);
            }

            // Binding ComboBox to our RuleType enumeration
            ComboBoxRulePartInput.ItemsSource = Enum.GetValues(typeof(RuleType)).Cast<RuleType>();
            ComboBoxRulePartInput.SelectedItem = RuleType.AnyDigit;
            ComboBoxMatchTypeInput.ItemsSource = Enum.GetValues(typeof(MatchType)).Cast<MatchType>();
            ComboBoxMatchTypeInput.SelectedItem = MatchType.ExactMatch;

            EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
            EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];

            TextBoxNameYourRuleInput.TextChanged += TextBoxNameYourRuleInput_TextChanged;
            TextBoxOutputParameterNameInput.TextChanged += TextBoxOutputParameterNameInput_TextChanged;

            RegexRuleParts.CollectionChanged += UpdateExampleText;
            ButtonTest.Click += UpdateExampleText;

            // Gives us the ability to close the window with the Esc kay
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };

            ButtonTest.IsEnabled = RegexRuleParts.Count > 0;
            if(!EditingExistingRule) TextBoxNameYourRuleInput.Focus();
        }
        private void ScrollViewerRuleParts_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ButtonAddRulePart_Click(object sender, RoutedEventArgs e)
        {
            RegexRulePart regexRulePart = RulePartServices.CreateRegexRulePart((RuleType) ComboBoxRulePartInput.SelectedItem);
            RegexRuleParts.Add(regexRulePart);
            TextBlockExample.Text = RegexAssembly.GenerateRandomExample(RegexRuleParts);
            ListBoxRuleParts.SelectedItem = regexRulePart;
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            UserFeedbackText = InputValidationServices.ReturnUserFeedback(RuleName, OutputParameterObject.ParameterObjectName, RegexRuleParts);
            TextBoxUserFeedback.Visibility = string.IsNullOrEmpty(UserFeedbackText) ? Visibility.Hidden : Visibility.Visible;
            
            // Helper method to read the static UI properties and turn them into a RegexRule
            RegexRule CreateRegexRuleFromUserInputs()
            {
                if (EditingExistingRule)
                {
                    // The rule already exists and is being edited. We'll generate a new temporary rule from the inputs to use as we transfer values across.
                    RegexRule regexRule = RegexRule.Create(DocumentGuid, ExistingRuleGuid);
                    regexRule.RuleName = RuleName;
                    regexRule.TargetCategoryIds = TargetCategoryIds;
                    regexRule.TrackingParameterObject = TrackingParameterObject;
                    regexRule.OutputParameterObject = OutputParameterObject;
                    regexRule.RegexRuleParts = RegexRuleParts;
                    regexRule.RegexString = RegexAssembly.AssembleRegexString(regexRule);
                    regexRule.MatchType = MatchType;
                    regexRule.IsFrozen = IsFrozen;
                    return regexRule;
                }
                else
                {
                    RegexRule regexRule = RegexRule.Create(DocumentGuid);
                    regexRule.RuleName = RuleName;
                    regexRule.TargetCategoryIds = TargetCategoryIds;
                    regexRule.TrackingParameterObject = TrackingParameterObject;
                    regexRule.OutputParameterObject = OutputParameterObject;
                    regexRule.RegexRuleParts = RegexRuleParts;
                    regexRule.RegexString = RegexAssembly.AssembleRegexString(regexRule);
                    regexRule.MatchType = MatchType;
                    regexRule.IsFrozen = IsFrozen;
                    return regexRule;
                }
            }

            if (EditingExistingRule)
            {
                // The rule already exists and is being edited. We'll generate a new temporary rule from the inputs to use as we transfer values across.
                RegexRule regexRule = CreateRegexRuleFromUserInputs();
                
                // Updates both the static cache and ExtensibleStorage.
                RegexRule.Update(DocumentGuid, ExistingRuleGuid, regexRule);
            }
            else
            {
                // This is a new rule, so we'll create a new rule object based on the static user input values. This will get saved.
                RegexRule regexRule = CreateRegexRuleFromUserInputs();

                // Saves rule to static cache and ExtensibleStorage
                RegexRule.Save(DocumentGuid, regexRule);
            }
            Close();
        }
        private void ButtonDeleteRegexRulePart_OnClick(object sender, RoutedEventArgs e)
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
            RegexRulePart regexRulePart = ListBoxRuleParts.SelectedItem as RegexRulePart;
            int index = RegexRuleParts.IndexOf(regexRulePart);
            if (index <= 0) return;
            RegexRuleParts.RemoveAt(index);
            RegexRuleParts.Insert(index - 1, regexRulePart);
            ListBoxRuleParts.Focus();
            ListBoxRuleParts.SelectedItem = regexRulePart;
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            RegexRulePart regexRulePart = ListBoxRuleParts.SelectedItem as RegexRulePart;
            int index = RegexRuleParts.IndexOf(regexRulePart);
            if (index >= RegexRuleParts.Count) return;
            RegexRuleParts.RemoveAt(index);
            RegexRuleParts.Insert(index + 1, regexRulePart);
            ListBoxRuleParts.Focus();
            ListBoxRuleParts.SelectedItem = regexRulePart;
        }
        private void UpdateExampleText(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (RegexRuleParts.Count <= 0)
            {
                ButtonTest.IsEnabled = false;
                TextBlockExample.Text = "Add rule parts to generate examples.";
                return;
            }
            ButtonTest.IsEnabled = true;
            TextBlockExample.Visibility = Visibility.Visible;
            TextBlockExample.Text = RegexAssembly.GenerateRandomExample(RegexRuleParts);
        }
        private void UpdateExampleText(object sender, RoutedEventArgs e)
        {
            if (RegexRuleParts.Count <= 0)
            {
                ButtonTest.IsEnabled = false;
                TextBlockExample.Text = "Add rule parts to generate examples.";
                return;
            }
            ButtonTest.IsEnabled = true;
            TextBlockExample.Visibility = Visibility.Visible;
            TextBlockExample.Text = RegexAssembly.GenerateRandomExample(RegexRuleParts);
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
        private void ScrollViewerCategories_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CategoryObject observableObject in TargetCategoryIds) { observableObject.IsChecked = true; }
        }
        private void ButtonSelectNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (CategoryObject observableObject in TargetCategoryIds) { observableObject.IsChecked = false; }
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
                NumberCategoriesSelected = TargetCategoryIds.Count(x => x.IsChecked);
            }
            void UpdateTrackingParameterSelectionComboBox()
            {
                ParameterObject currentParameterObject = ComboBoxTrackingParameterInput.SelectedItem as ParameterObject;
                // Updates the ComboBox to let users select parameter
                PossibleTrackingParameters = ParameterServices.GetParametersOfCategories(DocumentGuid, TargetCategoryIds);
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
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
 