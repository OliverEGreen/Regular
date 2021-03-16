using System.Windows;
using Regular.UI.SelectElements.Model;
using Regular.UI.SelectElements.ViewModel;

namespace Regular.UI.SelectElements.View
{
    public partial class SelectElementsView
    {
        public SelectElementsViewModel SelectElementsViewModel { get; set; }
        public SelectElementsView(SelectElementsInfo selectElementsInfo)
        {
            InitializeComponent();
            SelectElementsViewModel = new SelectElementsViewModel(selectElementsInfo);
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
