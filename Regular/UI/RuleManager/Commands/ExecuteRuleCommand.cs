using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Regular.Enums;
using Regular.Models;
using Regular.UI.RuleEditor.View;
using Regular.UI.RuleManager.ViewModel;
using Regular.Utilities;

namespace Regular.UI.RuleManager.Commands
{
    public class ExecuteRuleCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public ExecuteRuleCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return ruleManagerViewModel.SelectedRegexRule != null;
        }

        public void Execute(object parameter)
        {
            ruleManagerViewModel.RuleValidationOutputs.Clear();
            
            List<Element> targetedElements = RuleExecutionUtils.GetTargetedElements
            (
                ruleManagerViewModel.DocumentGuid,
                ruleManagerViewModel.SelectedRegexRule.RuleGuid
            );

            if (targetedElements.Count < 1) return;

            ruleManagerViewModel.TrackingParameterName = ruleManagerViewModel.SelectedRegexRule.TrackingParameterObject.ParameterObjectName;
            ruleManagerViewModel.ProgressBarTotalNumberElementsProcessed = 0;
            ruleManagerViewModel.ProgressBarTotalNumberOfElements = targetedElements.Count;
            ruleManagerViewModel.ProgressBarPercentage = 0;

            foreach (Element element in targetedElements)
            {
                RuleValidationInfo ruleValidationInfo = new RuleValidationInfo
                {
                    Element = element,
                    DocumentGuid = ruleManagerViewModel.DocumentGuid,
                    RegexRule = ruleManagerViewModel.SelectedRegexRule
                };
                ruleManagerViewModel.RuleValidationOutputs.Add(new RuleValidationOutput(ruleValidationInfo));
                ruleManagerViewModel.UpdateProgressBar();
            }

            ruleManagerViewModel.ExportReportEnabled = true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}