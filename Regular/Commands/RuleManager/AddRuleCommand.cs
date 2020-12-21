using System;
using System.Windows.Input;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class AddRuleCommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public AddRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute() => true;

        public void Execute()
        {
            // No second argument is provided, we'll work with a brand new rule
            Views.RuleEditor ruleEditor = new Views.RuleEditor(ruleManagerViewModel.DocumentGuid);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
