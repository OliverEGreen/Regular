using System;
using System.Windows.Input;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Commands
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
            RegexRulePart regexRulePart = new RegexRulePart(ruleEditorViewModel.SelectedRegexRulePartType);
            ruleEditorViewModel.StagingRule.RegexRuleParts.Add(regexRulePart);
            ruleEditorViewModel.CompliantExample = RegexAssembly.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
