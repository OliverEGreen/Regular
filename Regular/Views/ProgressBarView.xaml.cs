using Regular.Models;
using Regular.ViewModels;

namespace Regular.Views
{
    public partial class ProgressBarView
    {
        public ProgressBarViewModel ProgressBarViewModel { get; }

        public ProgressBarView(RegexRule regexRule, int totalNumberItems)
        {
            InitializeComponent();
            ProgressBarViewModel = new ProgressBarViewModel(regexRule, totalNumberItems)
            {
                ProgressBar = ReportProgressBar
            };
            DataContext = ProgressBarViewModel;
        }
    }
}