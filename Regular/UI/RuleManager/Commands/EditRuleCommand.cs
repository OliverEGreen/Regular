using System;
using System.Windows.Input;
using Regular.Enums;
using Regular.Models;
using Regular.UI.RuleEditor.View;
using Regular.UI.RuleManager.ViewModel;

namespace Regular.UI.RuleManager.Commands
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
            return parameter is RegexRule;
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