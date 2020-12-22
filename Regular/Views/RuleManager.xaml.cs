using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Views
{
    public partial class RuleManager
    {
        public RuleManagerViewModel RuleManagerViewModel { get; set; }

        public RuleManager(string documentGuid)
        {
            InitializeComponent();
            RuleManagerViewModel = new RuleManagerViewModel(documentGuid);
            DataContext = RuleManagerViewModel;
            // Gives us the ability to close the window with the Esc kay
            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape) Close();
            };
        }

        private void RegexRulesScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e) => Close();

    }
}
