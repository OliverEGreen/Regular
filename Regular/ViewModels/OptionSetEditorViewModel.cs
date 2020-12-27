using System.Collections.ObjectModel;
using System.ComponentModel;
using Regular.Models;

namespace Regular.ViewModels
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

            Options.Add(new OptionObject { OptionValue = "Test1" });
            Options.Add(new OptionObject { OptionValue = "Test2" });
            Options.Add(new OptionObject { OptionValue = "Test3" });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
