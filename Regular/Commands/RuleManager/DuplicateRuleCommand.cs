using System;
using System.Windows.Input;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class DuplicateRuleCommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public DuplicateRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute => true;

        public void Execute()
        {
            Views.RuleEditor ruleEditor = new Views.RuleEditor(
                ruleManagerViewModel.DocumentGuid,
                RegexRule.Duplicate(ruleManagerViewModel.DocumentGuid, ruleManagerViewModel.SelectedRegexRule));
            ruleEditor.ShowDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
