using System;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.RuleManager.ViewModel;

namespace Regular.UI.RuleManager.Commands
{
    public class MoveRuleDownCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public MoveRuleDownCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (ruleManagerViewModel.RegexRules.Count < 1 ||
                ruleManagerViewModel.SelectedRegexRule == null) return false;
            RegexRule selectedRegexRule = ruleManagerViewModel.SelectedRegexRule;
            int index = ruleManagerViewModel.RegexRules.IndexOf(selectedRegexRule);
            return index < ruleManagerViewModel.RegexRules.Count - 1;
        }

        public void Execute(object parameter)
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
