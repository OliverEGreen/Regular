using System.Collections.ObjectModel;
using Regular.Commands.RuleManager;
using Regular.Models;
using Regular.Views;

namespace Regular.ViewModels
{
    public class RuleManagerViewModel : NotifyPropertyChangedBase
    {
        // The document we're editing rules within
        public string DocumentGuid { get; set; }
        
        private ObservableCollection<RegexRule> regexRules;
        public ObservableCollection<RegexRule> RegexRules
        {
            get => regexRules;
            set
            {
                regexRules = value;
                NotifyPropertyChanged();
            }
        }
        
        private RegexRule selectedRegexRule;
        public RegexRule SelectedRegexRule
        {
            get => selectedRegexRule;
            set
            {
                selectedRegexRule = value;
                NotifyPropertyChanged();
            }
        }

        // ICommands
        public AddRuleCommand AddRuleCommand { get; }
        public DeleteRuleCommand DeleteRuleCommand { get; }
        public EditRuleCommand EditRuleCommand { get; }
        public DuplicateRuleCommand DuplicateRuleCommand { get; }
        public MoveRuleDownCommand MoveRuleDownCommand { get; }
        public MoveRuleUpCommand MoveRuleUpCommand { get; }
        public TriggerRuleFrozenCommand TriggerRuleFrozenCommand { get; }

        public RuleManagerViewModel(string documentGuid)
        {
            DocumentGuid = documentGuid;
            RegexRules = RegularApp.RegexRuleCacheService.GetDocumentRules(DocumentGuid);

            AddRuleCommand = new AddRuleCommand(this);
            DeleteRuleCommand = new DeleteRuleCommand(this);
            EditRuleCommand = new EditRuleCommand(this);
            DuplicateRuleCommand = new DuplicateRuleCommand(this);
            MoveRuleUpCommand = new MoveRuleUpCommand(this);
            MoveRuleDownCommand = new MoveRuleDownCommand(this);
            TriggerRuleFrozenCommand = new TriggerRuleFrozenCommand(this);
        }
    }
}
