using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Regular.Enums;
using Regular.Models;
using Regular.UI.RuleManager.ViewModel;
using Regular.Utilities;
using Color = System.Windows.Media.Color;
using Grid = System.Windows.Controls.Grid;
using Rectangle = System.Windows.Shapes.Rectangle;

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
        
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid)) return;
            UIElementCollection children = grid.Children;
            foreach (UIElement uiElement in children)
            {
                switch (uiElement)
                {
                    case Rectangle rectangle:
                    {
                        Color rolloverColor = (Color)ColorConverter.ConvertFromString("#CCCCCC");
                        rectangle.Fill = new SolidColorBrush(rolloverColor);
                        break;
                    }
                    case TextBox textBox:
                    {
                        Color rolloverColor = (Color)ColorConverter.ConvertFromString("#CCCCCC");
                        textBox.Background = new SolidColorBrush(rolloverColor);
                        break;
                    }
                }
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid)) return;
            UIElementCollection children = grid.Children;
            foreach (UIElement uiElement in children)
            {
                switch (uiElement)
                {
                    case Rectangle rectangle:
                    {
                        Color rolloutColor = (Color)ColorConverter.ConvertFromString("#E5E5E5");
                        rectangle.Fill = new SolidColorBrush(rolloutColor);
                        break;
                    }
                    case TextBox textBox:
                    {
                        Color rolloverColor = (Color)ColorConverter.ConvertFromString("#E5E5E5");
                        textBox.Background = new SolidColorBrush(rolloverColor);
                        break;
                    }
                }
            }
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
                    if (ruleValidationOutput.RuleValidationResult != RuleValidationResult.Valid) return;
                    ruleValidationOutput.CompliantExample = "";
                }
            }
        }

        private void ListBoxItem_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid)) return;
            if (!(grid.DataContext is RegexRule gridRegexRule)) return;

            ListBox listBox = VisualTreeUtils.FindParent<ListBox>(grid);
            if (listBox == null) return;
            
            if(listBox.SelectedItem != null)
            {
                RegexRule regexRule = listBox.SelectedItem as RegexRule;
                if (regexRule.RuleGuid == gridRegexRule.RuleGuid) return;
            }

            UIElementCollection children = grid.Children;
            foreach (UIElement uiElement in children)
            {
                switch (uiElement)
                {
                    case Rectangle rectangle:
                        {
                            Color rolloverColor = (Color)ColorConverter.ConvertFromString("#CCCCCC");
                            rectangle.Fill = new SolidColorBrush(rolloverColor);
                            break;
                        }
                    case TextBox textBox:
                        {
                            Color rolloverColor = (Color)ColorConverter.ConvertFromString("#CCCCCC");
                            textBox.Background = new SolidColorBrush(rolloverColor);
                            break;
                        }
                }
            }
        }

        private void ListBoxItem_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid)) return;
            if (!(grid.DataContext is RegexRule gridRegexRule)) return;

            ListBox listBox = VisualTreeUtils.FindParent<ListBox>(grid);
            if (listBox == null) return;

            if (listBox.SelectedItem != null)
            {
                RegexRule regexRule = listBox.SelectedItem as RegexRule;
                if (regexRule.RuleGuid == gridRegexRule.RuleGuid) return;
            }

            UIElementCollection children = grid.Children;
            foreach (UIElement uiElement in children)
            {
                switch (uiElement)
                {
                    case Rectangle rectangle:
                        {
                            Color rolloutColor = (Color)ColorConverter.ConvertFromString("#E5E5E5");
                            rectangle.Fill = new SolidColorBrush(rolloutColor);
                            break;
                        }
                    case TextBox textBox:
                        {
                            Color rolloutColor = (Color)ColorConverter.ConvertFromString("#E5E5E5");
                            textBox.Background = new SolidColorBrush(rolloutColor);
                            break;
                        }
                }
            }
        }


        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Grid senderGrid)) return;
            if (!(senderGrid.DataContext is RegexRule senderRegexRule)) return;

            ListBox listBox = VisualTreeUtils.FindParent<ListBox>(senderGrid);
            if (listBox == null) return;
            List<Grid> grids = listBox.FindVisualChildren<Grid>().ToList();

            foreach(Grid grid in grids)
            {
                if (!(grid.DataContext is RegexRule regexRule)) continue;
                if (regexRule.RuleGuid == senderRegexRule.RuleGuid)
                {
                    UIElementCollection children = grid.Children;
                    foreach (UIElement uiElement in children)
                    {
                        switch (uiElement)
                        {
                            case Rectangle rectangle:
                            {
                                Color rolloverColor = (Color)ColorConverter.ConvertFromString("#7CCEF4");
                                rectangle.Fill = new SolidColorBrush(rolloverColor);
                                break;
                            }
                            case TextBox textBox:
                            {
                                Color rolloverColor = (Color)ColorConverter.ConvertFromString("#7CCEF4");
                                textBox.Background = new SolidColorBrush(rolloverColor);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    UIElementCollection children = grid.Children;
                    foreach (UIElement uiElement in children)
                    {
                        switch (uiElement)
                        {
                            case Rectangle rectangle:
                            {
                                Color rolloutColor = (Color)ColorConverter.ConvertFromString("#E5E5E5");
                                rectangle.Fill = new SolidColorBrush(rolloutColor);
                                break;
                            }
                            case TextBox textBox:
                            {
                                Color rolloutColor = (Color)ColorConverter.ConvertFromString("#E5E5E5");
                                textBox.Background = new SolidColorBrush(rolloutColor);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
