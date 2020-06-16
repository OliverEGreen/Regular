using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Regular.Services;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using TextBox = System.Windows.Controls.TextBox;
using Visibility = System.Windows.Visibility;

namespace Regular.View
{
    public partial class RuleEditor: INotifyPropertyChanged
    {
        private static Document Document { get; set; }
        private static string DocumentGuid { get; set; }

        private string name;
        private ObservableCollection<ObservableObject> targetCategoryIds;
        private List<string> possibleTrackingParameters;
        private ObservableCollection<RegexRulePart> regexRuleParts;
        private string trackingParameterName; // Eventually, this should be some kind of ID
        private string outputParameterName; // This should also be an ID
        private MatchType matchType;
        private int numberCategoriesSelected;
        private bool EditingExistingRule { get; set; }
        private string ExistingRuleGuid { get; set; }

        public string RuleName
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged("RuleName");
            }
        }
        public ObservableCollection<ObservableObject> TargetCategoryIds
        {
            get => targetCategoryIds;
            set
            {
                targetCategoryIds = value;
                NotifyPropertyChanged("TargetCategoryIds");
            }
        }
        public List<string> PossibleTrackingParameters
        {
            get => possibleTrackingParameters;
            set
            {
                possibleTrackingParameters = value;
                NotifyPropertyChanged("PossibleTrackingParameters");
            }
        }
        public string TrackingParameterName
        {
            get => trackingParameterName;
            set
            {
                trackingParameterName = value;
                NotifyPropertyChanged("TrackingParameterName");
            }
        }
        public string OutputParameterName
        {
            get => outputParameterName;
            set
            {
                outputParameterName = value;
                NotifyPropertyChanged("OutputParameterName");
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
       
        public RuleEditor(string documentGuid, RegexRule inputRegexRule)
        {
            InitializeComponent();
            DataContext = this;
            InitializeRuleEditor(documentGuid, inputRegexRule);
        }
        private void InitializeRuleEditor(string documentGuid, RegexRule inputRegexRule)
        {
            EditingExistingRule = RegexRule.GetDocumentRegexRuleGuids(documentGuid).Contains(inputRegexRule.Guid);
            if(EditingExistingRule) ExistingRuleGuid = inputRegexRule.Guid;

            DocumentGuid = documentGuid;
            RuleName = inputRegexRule.Name;
            TargetCategoryIds = inputRegexRule.TargetCategoryIds;
            RegexRuleParts = inputRegexRule.RegexRuleParts;
            TrackingParameterName = inputRegexRule.TrackingParameterName;
            OutputParameterName = inputRegexRule.OutputParameterName;
            MatchType = inputRegexRule.MatchType;
            NumberCategoriesSelected = targetCategoryIds.Count(x => x.IsChecked);
            
            Title = EditingExistingRule ? $"Editing Rule: {Name}" : "Creating New Rule";
            TextBoxOutputParameterNameInput.IsEnabled = ! EditingExistingRule;

            // Binding ComboBox to our RuleType enumeration
            ComboBoxRulePartInput.ItemsSource = Enum.GetValues(typeof(RuleType)).Cast<RuleType>();
            ComboBoxRulePartInput.SelectedItem = RuleType.AnyDigit;
            ComboBoxMatchTypeInput.ItemsSource = Enum.GetValues(typeof(MatchType)).Cast<MatchType>();
            ComboBoxMatchTypeInput.SelectedItem = MatchType.ExactMatch;

            EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
            EllipseOutputParameterNameInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];

            TextBoxNameYourRuleInput.TextChanged += TextBoxNameYourRuleInput_TextChanged;
            TextBoxNameYourRuleInput.TextChanged += DisplayUserFeedback;
            TextBoxOutputParameterNameInput.TextChanged += TextBoxOutputParameterNameInput_TextChanged;
            TextBoxOutputParameterNameInput.TextChanged += DisplayUserFeedback;

            RegexRuleParts.CollectionChanged += UpdateExampleText;
            RegexRuleParts.CollectionChanged += DisplayUserFeedback;
            TargetCategoryIds.CollectionChanged += DisplayUserFeedback;
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
            RegexRuleParts.Add(RulePartServices.CreateRegexRulePart((RuleType)ComboBoxRulePartInput.SelectedItem));
            TextBlockExample.Text = RegexAssembly.GenerateRandomExample(RegexRuleParts);
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Must run all validation before going beyond this point!

            if (!EditingExistingRule)
            {
                RegexRule regexRule = RegexRule.Create(DocumentGuid);
                regexRule.Name = RuleName;
                regexRule.TargetCategoryIds = TargetCategoryIds;
                regexRule.TrackingParameterName = TrackingParameterName;
                regexRule.OutputParameterName = OutputParameterName;
                regexRule.MatchType = MatchType;
                regexRule.RegexRuleParts = RegexRuleParts;
                regexRule.RegexString = RegexAssembly.AssembleRegexString(regexRule.RegexRuleParts);

                RegexRule.Save(DocumentGuid, regexRule);
                ExtensibleStorageServices.SaveRegexRuleToExtensibleStorage(DocumentGuid, regexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, regexRule.Guid);
                
                // If a new rule, a new project parameter needs to be created.
                ParameterServices.CreateProjectParameter(Document, regexRule.OutputParameterName, ParameterType.Text, regexRule.TargetCategoryIds.Select(x => x.Id).ToList(), BuiltInParameterGroup.PG_IDENTITY_DATA, true);
            }
            else
            {
                // The rule already exists and is being edited. We'll generate a new temporary rule from the inputs to use as we transfer values across.
                RegexRule regexRule = RegexRule.Create(DocumentGuid, ExistingRuleGuid);
                regexRule.Name = RuleName;
                regexRule.TargetCategoryIds = TargetCategoryIds;
                regexRule.TrackingParameterName = TrackingParameterName;
                regexRule.OutputParameterName = OutputParameterName;
                regexRule.MatchType = MatchType;
                regexRule.RegexRuleParts = RegexRuleParts;
                regexRule.RegexString = RegexAssembly.AssembleRegexString(regexRule.RegexRuleParts);
                
                // We update both the static cache and the entity saved in ExtensibleStorage.
                RegexRule.Update(DocumentGuid, ExistingRuleGuid, regexRule);
                ExtensibleStorageServices.UpdateRegexRuleInExtensibleStorage(DocumentGuid, ExistingRuleGuid, regexRule);
                DynamicModelUpdateServices.RegisterRegexRule(DocumentGuid, ExistingRuleGuid);
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
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = RegexRuleParts.IndexOf(regexRulePart);

            if (index <= 0) return;
            RegexRuleParts.RemoveAt(index);
            RegexRuleParts.Insert(index - 1, regexRulePart);
        }
        private void ReorderDownButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RegexRulePart regexRulePart = (RegexRulePart)button.DataContext;
            int index = RegexRuleParts.IndexOf(regexRulePart);

            if (index >= RegexRuleParts.Count) return;
            RegexRuleParts.RemoveAt(index);
            RegexRuleParts.Insert(index + 1, regexRulePart);
        }
        private void DisplayUserFeedback(object sender, RoutedEventArgs e)
        {
            string userFeedback = InputValidationServices.ReturnUserFeedback(TextBoxNameYourRuleInput.Text, TextBoxOutputParameterNameInput.Text, RegexRuleParts);
            if (string.IsNullOrEmpty(userFeedback))
            {
                TextBoxUserFeedback.Visibility = Visibility.Hidden;
                return;
            }
            TextBoxUserFeedback.Visibility = Visibility.Visible;
            TextBoxUserFeedback.Text = userFeedback;
        }
        private void DisplayUserFeedback(object sender, NotifyCollectionChangedEventArgs e)
        {
            string userFeedback = InputValidationServices.ReturnUserFeedback(TextBoxNameYourRuleInput.Text, TextBoxOutputParameterNameInput.Text, RegexRuleParts);
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
            Button button = sender as Button;
            button.Content = ColumnCategories.Width == new GridLength(250) ? "Select Categories" : "Hide Categories";
            ColumnCategories.Width = ColumnCategories.Width == new GridLength(250) ? new GridLength(0) : new GridLength(250);
        }
        private void ScrollViewerCategories_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (ObservableObject observableObject in TargetCategoryIds) { observableObject.IsChecked = true; }
        }
        private void ButtonSelectNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (ObservableObject observableObject in TargetCategoryIds) { observableObject.IsChecked = false; }
        }
        private void ButtonTest_OnClick(object sender, RoutedEventArgs e)
        {
            if(this.RowExamples.Height != new GridLength(20)) this.RowExamples.Height = new GridLength(20);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void UpdateCheckedBoxesCount(object sender, RoutedEventArgs e)
        {
            NumberCategoriesSelected = TargetCategoryIds.Count(x => x.IsChecked);
            List<ElementId> categoryIds = TargetCategoryIds
                .Where(x => x.IsChecked)
                .Select(x => Convert.ToInt32(x.Id))
                .Select(x => new ElementId(x))
                .ToList();
            PossibleTrackingParameters = ParameterServices.GetParametersOfCategories(DocumentGuid, categoryIds);
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
    }
}
 