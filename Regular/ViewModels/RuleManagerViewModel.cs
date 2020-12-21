using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Regular.Annotations;
using Regular.Commands.RuleManager;
using Regular.Models;

namespace Regular.ViewModels
{
    public class RuleManagerViewModel : INotifyPropertyChanged
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
                NotifyPropertyChanged("RegexRules");
            }
        }
        
        private RegexRule selectedRegexRule;
        public RegexRule SelectedRegexRule
        {
            get => selectedRegexRule;
            set
            {
                selectedRegexRule = value;
                NotifyPropertyChanged("SelectedRegexRule");
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
            RegexRules = Models.RegexRules.AllRegexRules[documentGuid];

            AddRuleCommand = new AddRuleCommand(this);
            DeleteRuleCommand = new DeleteRuleCommand(this);
            EditRuleCommand = new EditRuleCommand(this);
            DuplicateRuleCommand = new DuplicateRuleCommand(this);
            MoveRuleUpCommand = new MoveRuleUpCommand(this);
            MoveRuleDownCommand = new MoveRuleDownCommand(this);
            TriggerRuleFrozenCommand = new TriggerRuleFrozenCommand(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
