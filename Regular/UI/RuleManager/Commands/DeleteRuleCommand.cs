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
            if (!(parameter is RegexRule regexRule)) return;

            ConfirmationDialogView confirmationDialogView = new ConfirmationDialogView
            (
                new ConfirmationDialog.Model.ConfirmationDialogInfo
                {
                    Title = $"Delete { regexRule.RuleName }?",
                    Header = $"Warning: Deleted Rules Cannot Be Retrieved",
                    Body = "Please confirm that you would like to delete this rule."
                }
            );
            confirmationDialogView.ShowDialog();
            if (confirmationDialogView.DialogResult != true) return;

            RegexRule.Delete(ruleManagerViewModel.DocumentGuid, regexRule);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}