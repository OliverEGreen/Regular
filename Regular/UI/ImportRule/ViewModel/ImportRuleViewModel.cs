using Regular.UI.ImportRule.Commands;
using Regular.UI.ImportRule.Enums;
using Regular.UI.ImportRule.Model;

namespace Regular.UI.ImportRule.ViewModel
{
    public class ImportRuleViewModel : NotifyPropertyChangedBase
    {
        public ImportRuleInfo ImportRuleInfo { get; set; }
        public string WarningHeader { get; set; }
        public string WarningBody { get; set; }

        public OverrideMode OverrideMode = OverrideMode.None;

        public ReplaceAllCommand ReplaceAllCommand { get; }
        public ReplaceRuleCommand ReplaceRuleCommand { get; }
        public RenameAllCommand RenameAllCommand { get; }
        public RenameRuleCommand RenameRuleCommand { get; }
        public SkipAllCommand SkipAllCommand { get; }
        public SkipRuleCommand SkipRuleCommand { get; }

        
        public ImportRuleViewModel(ImportRuleInfo importRuleInfo)
        {
            ImportRuleInfo = importRuleInfo;

            WarningHeader = $"Rule '{importRuleInfo.NewRegexRule.RuleName}' Already Exists";
            WarningBody = "Do you want to update the existing rule, rename it, or skip this import?";

            ReplaceAllCommand = new ReplaceAllCommand(this);
            ReplaceRuleCommand = new ReplaceRuleCommand(this);
            RenameAllCommand = new RenameAllCommand(this);
            RenameRuleCommand = new RenameRuleCommand(this);
            SkipAllCommand = new SkipAllCommand(this);
            SkipRuleCommand = new SkipRuleCommand(this);
        }
    }
}
