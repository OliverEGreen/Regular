using System.Collections.ObjectModel;
using Regular.Models;

namespace Regular.UI.DebugWindow.ViewModel
{
    public class DebugWindowViewModel : NotifyPropertyChangedBase
    {
        private ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();

        public ObservableCollection<RegexRule> RegexRules
        {
            get => regexRules;
            set
            {
                regexRules = value;
                NotifyPropertyChanged();
            }
        }

        public DebugWindowViewModel(ObservableCollection<RegexRule> regexRules)
        {
            RegexRules = regexRules;
        }
    }
}
