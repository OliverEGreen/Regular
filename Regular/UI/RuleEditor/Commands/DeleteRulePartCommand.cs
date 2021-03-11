using System;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.RuleEditor.ViewModel;

namespace Regular.UI.RuleEditor.Commands
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
            if (!(parameter is IRegexRulePart regexRulePart)) return;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(regexRulePart);
            ruleEditorViewModel.StagingRule.RegexRuleParts.Remove(regexRulePart);
            ruleEditorViewModel.GenerateCompliantExampleCommand.Execute(null);
            int newIndex = index > ruleEditorViewModel.StagingRule.RegexRuleParts.Count - 1 ? index - 1 : index;
            if (newIndex < 0)
            {
                ruleEditorViewModel.SelectedRegexRulePart = null;
                return;
            }
            ruleEditorViewModel.SelectedRegexRulePart = ruleEditorViewModel.StagingRule.RegexRuleParts[newIndex];
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
