using System.ComponentModel;

namespace Regular.Models
{
    public class OptionObject : INotifyPropertyChanged
    {
        private string optionValue;

        public string OptionValue
        {
            get => optionValue;
            set
            {
                optionValue = value;
                NotifyPropertyChanged("OptionValue");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
