﻿using System;
using System.Windows.Input;
using Regular.Models;
using Regular.Utilities;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class AddRulePartCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public AddRulePartCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            IRegexRulePart regexRulePart = RegexRulePart.Create(ruleEditorViewModel.SelectedRuleType);
            ruleEditorViewModel.SelectedRegexRulePart = regexRulePart;
            ruleEditorViewModel.StagingRule.RegexRuleParts.Add(regexRulePart);
            ruleEditorViewModel.CompliantExample = RegexAssemblyUtils.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);
            ruleEditorViewModel.UpdateRegexStringCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
