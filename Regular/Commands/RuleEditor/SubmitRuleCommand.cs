using System;
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
                RegexRule existingRegexRule = ruleEditorViewModel.InputRule;
                RegexRule newRegexRule = ruleEditorViewModel.StagingRule;

                // Takes a newly-generated RegexRule object and sets an existing rules values to match
                // To be used when updating an existing rule from the Rule Editor

                existingRegexRule.RuleName = newRegexRule.RuleName;
                existingRegexRule.TargetCategoryObjects = newRegexRule.TargetCategoryObjects;
                existingRegexRule.TrackingParameterObject = newRegexRule.TrackingParameterObject;
                existingRegexRule.OutputParameterObject = newRegexRule.OutputParameterObject;
                existingRegexRule.MatchType = newRegexRule.MatchType;
                existingRegexRule.RegexRuleParts = newRegexRule.RegexRuleParts;
                existingRegexRule.RegexString = newRegexRule.RegexString;
                existingRegexRule.IsFrozen = newRegexRule.IsFrozen;
                existingRegexRule.UpdaterId = newRegexRule.UpdaterId;

                // Need to check if existingRegexRule is in ExtensibleStorage or not.
                ExtensibleStorageUtils.UpdateRegexRuleInExtensibleStorage(ruleEditorViewModel.DocumentGuid, existingRegexRule.RuleGuid, newRegexRule);
                DmTriggerUtils.UpdateTrigger(ruleEditorViewModel.DocumentGuid, existingRegexRule);
            }
            else
            {
                // Saves rule to static cache and ExtensibleStorage
                RegularApp.RegexRuleCacheService.AddRule(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.StagingRule);
                ExtensibleStorageUtils.SaveRegexRuleToExtensibleStorage(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.StagingRule);
                DmTriggerUtils.AddTrigger(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.StagingRule);

                // TODO: Check this rule is created as we want
                ParameterUtils.CreateProjectParameter(ruleEditorViewModel.DocumentGuid, ruleEditorViewModel.StagingRule.OutputParameterObject.ParameterObjectName, ruleEditorViewModel.StagingRule.TargetCategoryObjects);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
