using System;
using System.Windows.Input;
using Regular.Enums;
using Regular.Models;
using Regular.UI.RuleEditor.View;
using Regular.UI.RuleManager.ViewModel;

namespace Regular.UI.RuleManager.Commands
{
    public class AddRuleCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public AddRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            RuleEditorInfo ruleEditorInfo = new RuleEditorInfo
            {
                DocumentGuid = ruleManagerViewModel.DocumentGuid,
                RuleEditorType = RuleEditorType.CreateNewRule
            };
            
            new RuleEditorView(ruleEditorInfo).ShowDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
