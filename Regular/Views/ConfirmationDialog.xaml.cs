using System.Windows;

namespace Regular.Views
{
    public partial class ConfirmationDialog : Window
    {
        public bool ConfirmDelete { get; set; } = false;
        public ConfirmationDialog()
        {
            InitializeComponent();
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e) => Close();
        
        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            ConfirmDelete = true;
            Close();
        }
    }
}
