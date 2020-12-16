using System;
using System.Windows.Input;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Commands
{
    public class DeleteRulePartCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public DeleteRulePartCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            return ruleEditorViewModel.StagingRule.RegexRuleParts.Count > 0;
        }

        public void Execute(object parameter)
        {
            if (ruleEditorViewModel.SelectedRegexRulePart == null) return;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(ruleEditorViewModel.SelectedRegexRulePart);
            ruleEditorViewModel.StagingRule.RegexRuleParts.Remove(ruleEditorViewModel.SelectedRegexRulePart);
            ruleEditorViewModel.CompliantExample = RegexAssembly.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);
            int newIndex = index > ruleEditorViewModel.StagingRule.RegexRuleParts.Count - 1 ? index - 1 : index;
            ruleEditorViewModel.SelectedRegexRulePart = ruleEditorViewModel.StagingRule.RegexRuleParts[newIndex];
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
