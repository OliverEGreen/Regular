using System;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class EditRuleCommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public EditRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute() => true;

        public void Execute(object parameter)
        {
            if (!(parameter is Button button)) return;
            // We open up the editor with the existing rule
            Views.RuleEditor ruleEditor = new Views.RuleEditor
            (
                ruleManagerViewModel.DocumentGuid,
                (RegexRule)button.DataContext
            );
            ruleEditor.ShowDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}