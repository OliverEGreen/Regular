using System;
using System.Windows.Input;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class MoveRuleUpCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public MoveRuleUpCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (ruleManagerViewModel.RegexRules.Count < 1 ||
                ruleManagerViewModel.SelectedRegexRule == null) return false;
            RegexRule selectedRegexRule = ruleManagerViewModel.SelectedRegexRule;
            int index = ruleManagerViewModel.RegexRules.IndexOf(selectedRegexRule);

            return index > 0;
        }

        public void Execute(object parameter)
        {
            RegexRule selectedRegexRule = ruleManagerViewModel.SelectedRegexRule;
            
            int index = ruleManagerViewModel.RegexRules.IndexOf(selectedRegexRule);
            ruleManagerViewModel.RegexRules.RemoveAt(index);
            ruleManagerViewModel.RegexRules.Insert(index - 1, selectedRegexRule);
            ruleManagerViewModel.SelectedRegexRule = selectedRegexRule;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
