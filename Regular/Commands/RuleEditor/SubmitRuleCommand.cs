using System;
using System.Windows.Input;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class SubmitRuleCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public SubmitRuleCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            // All validation logic for submitting a rule lives here
            
            RegexRule stagingRule = ruleEditorViewModel.StagingRule;
            
            // If any of the following cases is true, the new rule cannot be submitted
            if (
                stagingRule.RuleName.Length < 1 ||
                stagingRule.RegexRuleParts.Count < 1 ||
                stagingRule.OutputParameterObject == null ||
                string.IsNullOrWhiteSpace(stagingRule.RegexString) ||
                stagingRule.TargetCategoryObjects == null ||
                stagingRule.TargetCategoryObjects.Count < 1 ||
                stagingRule.TrackingParameterObject == null
            )
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            // This ICommand executing presumes that all validation logic in CanExecute has returned true

            if (ruleEditorViewModel.EditingExistingRule)
            {
                // Updates both the static cache and ExtensibleStorage.
                RegexRule.Update(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.InputRule, ruleEditorViewModel.StagingRule);
            }
            else
            {
                // Saves rule to static cache and ExtensibleStorage
                RegexRule.Save(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.StagingRule);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
