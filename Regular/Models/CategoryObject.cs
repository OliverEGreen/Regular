using System.ComponentModel;

namespace Regular.Models
{
    public class CategoryObject : INotifyPropertyChanged
    {
        // Using these to bind categories to a checkbox list
        // Ids get stored in ExtensibleStorage, Names are displayed to the user 
        // and IsChecked is used to save the checkbox state for each object
        
        private string categoryObjectName;
        private int categoryObjectId;
        private bool isChecked;
                        
        public string CategoryObjectName
        {
            get => categoryObjectName;
            set
            {
                categoryObjectName = value;
                NotifyPropertyChanged("RuleName");
            }
        }
        public int CategoryObjectId
        {
            get => categoryObjectId;
            set
            {
                categoryObjectId = value;
                NotifyPropertyChanged("CategoryObjectId");
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
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

