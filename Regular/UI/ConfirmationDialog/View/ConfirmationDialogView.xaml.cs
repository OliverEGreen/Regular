using Regular.UI.ConfirmationDialog.Model;
using Regular.UI.ConfirmationDialog.ViewModel;
using System.Windows;

namespace Regular.UI.ConfirmationDialog.View
{
    public partial class ConfirmationDialogView
    {
        public ConfirmationDialogViewModel ConfirmationDialogViewModel { get; set; }
        public ConfirmationDialogView(ConfirmationDialogInfo confirmationDialogInfo)
        {
            InitializeComponent();
            ConfirmationDialogViewModel = new ConfirmationDialogViewModel(confirmationDialogInfo);
            DataContext = ConfirmationDialogViewModel;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e) => Close();
        
        private void ButtonConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
