using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class MoveRuleDownCommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public MoveRuleDownCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute()
        {
            if (ruleManagerViewModel.RegexRules.Count < 1 ||
                ruleManagerViewModel.SelectedRegexRule == null) return false;
            RegexRule selectedRegexRule = ruleManagerViewModel.SelectedRegexRule;
            int index = ruleManagerViewModel.RegexRules.IndexOf(selectedRegexRule);
            return index < ruleManagerViewModel.RegexRules.Count - 1;
        }

        public void Execute()
        {
            RegexRule selectedRegexRule = ruleManagerViewModel.SelectedRegexRule;
            
            int index = ruleManagerViewModel.RegexRules.IndexOf(selectedRegexRule);
            ruleManagerViewModel.RegexRules.RemoveAt(index);
            ruleManagerViewModel.RegexRules.Insert(index + 1, selectedRegexRule);
            ruleManagerViewModel.SelectedRegexRule = selectedRegexRule;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
