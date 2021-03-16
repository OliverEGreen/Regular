using System.Windows;
using Regular.UI.InfoWindow.ViewModel;

namespace Regular.UI.InfoWindow.View
{
    public partial class InfoWindowView
    {
        public InfoWindowViewModel InfoWindowViewModel { get; }
        
        public InfoWindowView(string title, string headerText, string bodyText, bool isWarning)
        {
            InitializeComponent();
            InfoWindowViewModel = new InfoWindowViewModel(title, headerText, bodyText, isWarning);
            DataContext = InfoWindowViewModel;
            
            TextBlockBody.Style = isWarning ? (Style)FindResource("RegularTextBlockBodyWarning") : (Style)FindResource("RegularTextBlockBody");
            TextBlockHeader.Style = isWarning ? (Style) FindResource("RegularTextBlockHeaderWarning") : (Style)FindResource("RegularTextBlockHeader");
        }
        
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}