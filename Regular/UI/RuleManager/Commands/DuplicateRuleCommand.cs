using System;
using System.Windows.Input;
using Regular.Enums;
using Regular.Models;
using Regular.UI.RuleEditor.View;
using Regular.UI.RuleManager.ViewModel;

namespace Regular.UI.RuleManager.Commands
{
    public class DuplicateRuleCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public DuplicateRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter)
        {
            // Can duplicate any rule as long as it's selected
            return ruleManagerViewModel.SelectedRegexRule != null;
        }

        public void Execute(object parameter)
        {
            RuleEditorInfo ruleEditorInfo = new RuleEditorInfo
            {
                DocumentGuid = ruleManagerViewModel.DocumentGuid,
                RegexRule = RegexRule.Duplicate(ruleManagerViewModel.DocumentGuid, ruleManagerViewModel.SelectedRegexRule, false),
                RuleEditorType = RuleEditorType.DuplicateExistingRule
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
