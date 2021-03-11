using System.Collections.ObjectModel;
using System.ComponentModel;
using Regular.Models;

namespace Regular.UI.OptionSetEditor.ViewModel
{
    public class OptionSetEditorViewModel
    {
        private ObservableCollection<OptionObject> options;
        public ObservableCollection<OptionObject> Options
        {
            get => options;
            set
            {
                options = value;
                NotifyPropertyChanged("Options");
            }
        }
        public OptionSetEditorViewModel(IRegexRulePart regexRulePart)
        {
            Options = regexRulePart.Options;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
