using System;
using System.Linq;
using System.Windows.Input;
using Regular.Models;
using Regular.Utilities;
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

            bool ruleNameLengthValid = stagingRule.RuleName.Length > 0;
            bool regexRulePartsCountValid = stagingRule.RegexRuleParts.Count > 0;
            bool outputParameterObjectNameLengthValid = stagingRule.OutputParameterObject.ParameterObjectName.Length > 0;
            bool regexStringLengthValid = !string.IsNullOrWhiteSpace(stagingRule.RegexString);
            bool targetCategoryObjectCountValid = stagingRule.TargetCategoryObjects.Count(x => x.IsChecked) > 0;
            bool trackingParameterObjectIdValid = stagingRule.TrackingParameterObject.ParameterObjectId != -1;

            // If any of the following cases is true, the new rule cannot be submitted
            return (
                ruleNameLengthValid &&
                regexRulePartsCountValid &&
                outputParameterObjectNameLengthValid &&
                regexStringLengthValid &&
                targetCategoryObjectCountValid &&
                trackingParameterObjectIdValid
            );
        }

        public void Execute(object parameter)
        {
            // This ICommand executing presumes that all validation logic in CanExecute has returned true

            if (ruleEditorViewModel.EditingExistingRule)
            {
                RegexRule.Update(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.InputRule, ruleEditorViewModel.StagingRule);
            }
            else
            {
                RegexRule.Save(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.StagingRule);
            }
            
            // Once the rule has either been created or altered, validation can run against all affected elements
            RuleExecutionUtils.ExecuteRegexRule(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.StagingRule);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
