using System.Collections.ObjectModel;
using Regular.Models;
using Regular.UI.RuleManager.Commands;

namespace Regular.UI.RuleManager.ViewModel
{
    public class RuleManagerViewModel : NotifyPropertyChangedBase
    {
        public string DocumentGuid { get; set; }

        // Private Members & Default Values
        private ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();
        private RegexRule selectedRegexRule = null;
        private ObservableCollection<RuleValidationOutput> ruleValidationOutputs = new ObservableCollection<RuleValidationOutput>();


        // Public Properties and NotifyPropertyChanged
        public ObservableCollection<RegexRule> RegexRules
        {
            get => regexRules;
            set
            {
                regexRules = value;
                NotifyPropertyChanged();
            }
        }
        public RegexRule SelectedRegexRule
        {
            get => selectedRegexRule;
            set
            {
                selectedRegexRule = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<RuleValidationOutput> RuleValidationOutputs
        {
            get => ruleValidationOutputs;
            set
            {
                ruleValidationOutputs = value;
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
        }
    }
}
