using System.Windows;

namespace Regular.Views
{
    public partial class ConfirmationDialog
    {
        public bool ConfirmDelete { get; set; }
        public ConfirmationDialog()
        {
            InitializeComponent();
            ConfirmDelete = false;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e) => Close();
        
        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            ConfirmDelete = true;
            Close();
        }
    }
}
