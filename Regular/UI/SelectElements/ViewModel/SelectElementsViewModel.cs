using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Regular.UI.SelectElements.Commands;
using Regular.UI.SelectElements.Model;

namespace Regular.UI.SelectElements.ViewModel
{
    public class SelectElementsViewModel : NotifyPropertyChangedBase
    {
        // Private Members & Defaults
        private int numberElementsSelected = 0;
        private int numberElementsTotal = 0;
        private ObservableCollection<ObservableObject> inputObservableObjects = new ObservableCollection<ObservableObject>();
       
        
        
        // Public Properties & NotifyPropertyChanged
        public int NumberElementsSelected
        {
            get => numberElementsSelected;
            set
            {
                numberElementsSelected = value;
                NotifyPropertyChanged();
            }
        }
        public int NumberElementsTotal
        {
            get => numberElementsTotal;
            set
            {
                numberElementsTotal = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<ObservableObject> InputObservableObjects
        {
            get => inputObservableObjects;
            set
            {
                inputObservableObjects = value; 
                NotifyPropertyChanged();
            }
        }
        public string UserMessage { get; set; }
        
        
        // ICommands
        public SelectElementsCommand SelectElementsCommand { get; set; }
        public TriggerSelectAllElementsCommand TriggerSelectAllElementsCommand { get; set; }
        public TriggerSelectElementCommand TriggerSelectElementCommand { get; set; }
        
        public SelectElementsInfo SelectElementsInfo { get; }
        
        
        public void RefreshSelectedElementCount() => NumberElementsSelected = InputObservableObjects.Count(x => x.IsChecked);
        
        public SelectElementsViewModel(SelectElementsInfo selectElementsInfo)
        {
            SelectElementsInfo = selectElementsInfo;
            InputObservableObjects = selectElementsInfo.ObservableObjects;
            UserMessage = selectElementsInfo.UserMessage;
            SelectElementsCommand = new SelectElementsCommand(this);
            TriggerSelectAllElementsCommand = new TriggerSelectAllElementsCommand(this);
            TriggerSelectElementCommand = new TriggerSelectElementCommand(this);
            NumberElementsTotal = selectElementsInfo.ObservableObjects.Count;
        }
    }
}
