using System;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.ConfirmationDialog.View;
using Regular.UI.RuleManager.ViewModel;

namespace Regular.UI.RuleManager.Commands
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
            ConfirmationDialogView confirmationDialogView = new ConfirmationDialogView();
            confirmationDialogView.ShowDialog();
            if (confirmationDialogView.DialogResult != true) return;

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