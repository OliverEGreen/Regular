using Regular.UI.ImportRule.Model;
using Regular.UI.ImportRule.ViewModel;

namespace Regular.UI.ImportRule.View
{
    public partial class ImportRuleView
    {
        public ImportRuleViewModel ImportRuleViewModel { get; set; }
        public ImportRuleView(ImportRuleInfo importRuleInfo)
        {
            InitializeComponent();
            ImportRuleViewModel = new ImportRuleViewModel(importRuleInfo);
            DataContext = ImportRuleViewModel;
        }

        private void CloseNormally(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
