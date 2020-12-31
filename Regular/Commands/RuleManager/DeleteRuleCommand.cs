using System;
using System.Windows.Input;
using Regular.Models;
using Regular.Utilities;
using Regular.ViewModels;
using Regular.Views;

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
            // Asks user if they're sure before deleting anything
            ConfirmationDialog confirmationDialog = new ConfirmationDialog();
            confirmationDialog.ShowDialog();
            if (!confirmationDialog.ConfirmDelete) return;

            if (!(parameter is RegexRule regexRule)) return;
            RegexRule.Delete(ruleManagerViewModel.DocumentGuid, regexRule);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}