using System.Windows;
using AhmmTools.UI.SelectElements.Model;
using AhmmTools.UI.SelectElements.ViewModel;

namespace AhmmTools.UI.SelectElements.View
{
    public partial class SelectElementsView
    {
        public SelectElementsViewModel SelectElementsViewModel { get; set; }
        public SelectElementsView(SelectElementsInfo selectElementsInfo)
        {
            InitializeComponent();
            SelectElementsViewModel = new SelectElementsViewModel(selectElementsInfo);
            CountColumn.Visibility = selectElementsInfo.DisplayCount ? Visibility.Visible : Visibility.Collapsed;
            DataContext = SelectElementsViewModel;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e) => Close();
        
        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
