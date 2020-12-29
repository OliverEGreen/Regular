using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;
using Regular.Views;

namespace Regular.Commands.RuleEditor
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
            ConfirmationDialog confirmationDialog = new ConfirmationDialog();
            confirmationDialog.ShowDialog();
            if (!confirmationDialog.ConfirmDelete) return;
            if (!(parameter is IRegexRulePart regexRulePart)) return;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(regexRulePart);
            ruleEditorViewModel.StagingRule.RegexRuleParts.Remove(regexRulePart);
            ruleEditorViewModel.CompliantExample = RegexAssemblyService.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);
            ruleEditorViewModel.StagingRule.RegexString = RegexAssemblyService.AssembleRegexString(ruleEditorViewModel.StagingRule);
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
