using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.ViewModels;

namespace Regular.Views
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

    }
}
