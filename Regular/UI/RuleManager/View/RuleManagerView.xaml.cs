using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Regular.Enums;
using Regular.Models;
using Regular.UI.RuleManager.ViewModel;
using Regular.Utilities;

namespace Regular.UI.RuleManager.View
{
    public partial class RuleManagerView
    {
        public RuleManagerViewModel RuleManagerViewModel { get; set; }

        public RuleManagerView(string documentGuid)
        {
            InitializeComponent();
            RuleManagerViewModel = new RuleManagerViewModel(documentGuid);
            DataContext = RuleManagerViewModel;
            // Gives us the ability to close the window with the Esc kay
            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape) Close();
            };
            RegularApp.DialogShowing = true;
        }

        private void RegexRulesScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e) => Close();

        private void ButtonExecuteRule_OnClick(object sender, RoutedEventArgs e)
        {
            ReportParameterNameColumn.Header = RuleManagerViewModel.SelectedRegexRule.TrackingParameterObject.ParameterObjectName;
        }

        private void DataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!(e.EditingElement is TextBox textBox)) return;
            if (textBox.DataContext is RuleValidationOutput ruleValidationOutput)
            {
                // 1. Try to change actual parameter value
                Document document = RegularApp.DocumentCacheService.GetDocument(RuleManagerViewModel.DocumentGuid);
                Element element = document.GetElement(ruleValidationOutput.ElementId);
                RegexRule regexRule = RuleManagerViewModel.SelectedRegexRule;
                Parameter parameter = ParameterUtils.GetParameterById
                (
                    document, 
                    element,
                    regexRule.TrackingParameterObject.ParameterObjectId
                );
                if (parameter == null || parameter.IsReadOnly) return;
                
                using(Transaction transaction = new Transaction(document, $"DataSpec User Modifying Element {element.Id}"))
                {
                    transaction.Start();
                    parameter.Set(textBox.Text);
                    transaction.Commit();
                    
                    RuleValidationResult ruleValidationResult = RuleExecutionUtils.ExecuteRegexRule
                    (
                        RuleManagerViewModel.DocumentGuid,
                        regexRule.RuleGuid,
                        element
                    );

                    ruleValidationOutput.RuleValidationResult = ruleValidationResult;
                    ruleValidationOutput.ValidationText = ruleValidationResult.GetEnumDescription();
                    if (ruleValidationOutput.RuleValidationResult == RuleValidationResult.Valid)
                    {
                        ruleValidationOutput.CompliantExample = "";
                    }
                }
            }
        }
    }
}
