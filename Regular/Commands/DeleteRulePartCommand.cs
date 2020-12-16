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
            return true;
            //return ruleEditorViewModel.StagingRule.RegexRuleParts.Count > 0;
        }

        public void Execute(object parameter)
        {
            if (ruleEditorViewModel.SelectedRegexRulePart == null) return;
            ruleEditorViewModel.StagingRule.RegexRuleParts.Remove(ruleEditorViewModel.SelectedRegexRulePart);
            ruleEditorViewModel.CompliantExample = RegexAssembly.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
