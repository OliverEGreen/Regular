using System.ComponentModel;

namespace Regular.Models
{
    public class ParameterObject : INotifyPropertyChanged
    {
        private string parameterObjectName = "";
        private int parameterObjectId = -1;

        public string ParameterObjectName
        {
            get => parameterObjectName;
            set
            {
                parameterObjectName = value;
                NotifyPropertyChanged("ParameterObjectName");
            }
        }
        public int ParameterObjectId
        {
            get => parameterObjectId;
            set
            {
                parameterObjectId = value;
                NotifyPropertyChanged("ParameterObjectId");
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

