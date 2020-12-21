using System;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class DeleteRuleCommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public DeleteRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute()
        {
            // The delete button is inactive if there are no rules, or there is no selected rule
            return !(ruleManagerViewModel.RegexRules.Count < 1 || ruleManagerViewModel.SelectedRegexRule == null);
        }

        public void Execute(object parameter)
        {
            // TODO: Add in confirmation button before rule gets deleted forever
            if (!(parameter is Button button)) return;
            string regexRuleGuid = ((RegexRule)button.DataContext).RuleGuid;

            // Deleting both the cached RegexRule and the associated DataStorage object
            RegexRule.Delete(ruleManagerViewModel.DocumentGuid, regexRuleGuid);
            ExtensibleStorageServices.DeleteRegexRuleFromExtensibleStorage(ruleManagerViewModel.DocumentGuid, regexRuleGuid);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}