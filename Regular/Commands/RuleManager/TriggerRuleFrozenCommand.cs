using System;
using System.Windows.Input;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class TriggerRuleFrozenCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public TriggerRuleFrozenCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter)
        {
            // If there's no selected item, we return
            return ruleManagerViewModel.SelectedRegexRule != null;
        }

        public void Execute(object parameter)
        {
            ruleManagerViewModel.SelectedRegexRule.IsFrozen = !(ruleManagerViewModel.SelectedRegexRule.IsFrozen);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
