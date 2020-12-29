using System;
using System.Windows.Input;
using Regular.Models;
using Regular.Utilities;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class MoveRulePartUpCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public MoveRulePartUpCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            if (ruleEditorViewModel.SelectedRegexRulePart == null) return false;
            IRegexRulePart regexRulePart = ruleEditorViewModel.SelectedRegexRulePart;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(regexRulePart);
            return index > 0;
        }

        public void Execute(object parameter)
        {
            IRegexRulePart regexRulePart = ruleEditorViewModel.SelectedRegexRulePart;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(regexRulePart);
            
            ruleEditorViewModel.StagingRule.RegexRuleParts.RemoveAt(index);
            ruleEditorViewModel.StagingRule.RegexRuleParts.Insert(index - 1, regexRulePart);
            ruleEditorViewModel.SelectedRegexRulePart = ruleEditorViewModel.StagingRule.RegexRuleParts[index - 1];
            ruleEditorViewModel.StagingRule.RegexString = RegexAssemblyUtils.AssembleRegexString(ruleEditorViewModel.StagingRule);
            // ListBoxRuleParts.Focus();
            // ListBoxRuleParts.SelectedItem = regexRulePart;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
