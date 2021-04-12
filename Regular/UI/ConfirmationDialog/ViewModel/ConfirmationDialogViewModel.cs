using Regular.UI.ConfirmationDialog.Model;

namespace Regular.UI.ConfirmationDialog.ViewModel
{
    public class ConfirmationDialogViewModel : NotifyPropertyChangedBase
    {
        private string title = "";
        private string header = "";
        private string body = "";

        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        public string Header
        {
            get => header;
            set
            {
                header = value;
                NotifyPropertyChanged();
            }
        }

        public string Body
        {
            get => body;
            set
            {
                body = value;
                NotifyPropertyChanged();
            }
        }

        public ConfirmationDialogViewModel(ConfirmationDialogInfo confirmationDialogInfo)
        {
            Title = confirmationDialogInfo.Title;
            Header = confirmationDialogInfo.Header;
            Body = confirmationDialogInfo.Body;
        }
    }
}
