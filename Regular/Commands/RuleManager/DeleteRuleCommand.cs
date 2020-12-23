using System;
using System.Windows.Input;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class DeleteRuleCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public DeleteRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            // TODO: Add in confirmation button before rule gets deleted forever
            if (!(parameter is RegexRule regexRule)) return;

            // Deleting both the cached RegexRule and the associated DataStorage object
            RegexRule.Delete(ruleManagerViewModel.DocumentGuid, regexRule.RuleGuid);
            ExtensibleStorageServices.DeleteRegexRuleFromExtensibleStorage(ruleManagerViewModel.DocumentGuid, regexRule.RuleGuid);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}