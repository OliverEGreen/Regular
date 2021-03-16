namespace Regular.UI.InfoWindow.ViewModel
{
    public class InfoWindowViewModel : NotifyPropertyChangedBase
    {
        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        private string headerText;
        public string HeaderText
        {
            get => headerText;
            set
            {
                headerText = value;
                NotifyPropertyChanged();
            }
        }

        private string bodyText;
        public string BodyText
        {
            get => bodyText;
            set
            {
                bodyText = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsWarning { get; }

        public InfoWindowViewModel(string title, string headerText, string bodyText, bool isWarning)
        {
            Title = title;
            HeaderText = headerText;
            BodyText = bodyText;
            IsWarning = isWarning;
        }
    }
}