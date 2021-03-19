using System.Collections.ObjectModel;
using Regular.Models;
using Regular.UI.DebugWindow.ViewModel;

namespace Regular.UI.DebugWindow.View
{
    public partial class DebugWindowView
    {
        public DebugWindowViewModel DebugWindowViewModel { get; }
        public DebugWindowView(ObservableCollection<RegexRule> regexRules)
        {
            InitializeComponent();
            DebugWindowViewModel = new DebugWindowViewModel(regexRules);
            DataContext = DebugWindowViewModel;
        }
    }
}
