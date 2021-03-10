using System;
using System.Windows.Input;
using Regular.Enums;
using Regular.Models;
using Regular.Views;
using Regular.ViewModels;

namespace Regular.Commands.RuleManager
{
    public class EditRuleCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public EditRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }
        
        public bool CanExecute(object parameter)
        {
            if (!(parameter is RegexRule regexRule)) return false;
            
            // If the rule is frozen, it cannot be edited
            return !(regexRule.IsFrozen);
        }
        
        public void Execute(object parameter)
        {
            // We open up the editor with the existing rule
            RuleEditorInfo ruleEditorInfo = new RuleEditorInfo
            {
                DocumentGuid = ruleManagerViewModel.DocumentGuid,
                RegexRule = (RegexRule) parameter,
                RuleEditorType = RuleEditorType.EditingExistingRule
            };

            RuleEditorView ruleEditorView = new RuleEditorView(ruleEditorInfo);
            ruleEditorView.ShowDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}