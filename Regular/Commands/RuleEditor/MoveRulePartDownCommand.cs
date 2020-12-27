﻿using System;
using System.Windows.Input;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class MoveRulePartDownCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public MoveRulePartDownCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            if (ruleEditorViewModel.SelectedRegexRulePart == null) return false;
            IRegexRulePart regexRulePart = ruleEditorViewModel.SelectedRegexRulePart;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(regexRulePart);
            return index < ruleEditorViewModel.StagingRule.RegexRuleParts.Count - 1;
        }

        public void Execute(object parameter)
        {
            IRegexRulePart regexRulePart = ruleEditorViewModel.SelectedRegexRulePart;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(regexRulePart);

            ruleEditorViewModel.StagingRule.RegexRuleParts.RemoveAt(index);
            ruleEditorViewModel.StagingRule.RegexRuleParts.Insert(index + 1, regexRulePart);
            ruleEditorViewModel.SelectedRegexRulePart = ruleEditorViewModel.StagingRule.RegexRuleParts[index + 1];
            ruleEditorViewModel.StagingRule.RegexString = RegexAssemblyService.AssembleRegexString(ruleEditorViewModel.StagingRule);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}