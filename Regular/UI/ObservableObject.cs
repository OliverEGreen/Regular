using System.ComponentModel;

namespace Regular.UI
{
    public class ObservableObject : INotifyPropertyChanged
    {
        private string displayName;
        private bool isChecked;
        
        public object OriginalObject { get; set; }
        
        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }
        
        public ObservableObject(object obj)
        {
            OriginalObject = obj;
            IsChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
