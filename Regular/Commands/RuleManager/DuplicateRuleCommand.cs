using System;
using System.Windows.Input;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class DuplicateRuleCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public DuplicateRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter)
        {
            // Can duplicate any rule as long as it's selected
            return ruleManagerViewModel.SelectedRegexRule != null;
        }

        public void Execute(object parameter)
        {
            RegexRule duplicatedRegexRule = RegexRule.Duplicate(ruleManagerViewModel.DocumentGuid, ruleManagerViewModel.SelectedRegexRule, false);

            Views.RuleEditor ruleEditor = new Views.RuleEditor(ruleManagerViewModel.DocumentGuid, false, duplicatedRegexRule);
            ruleEditor.ShowDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
