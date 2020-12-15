using System;
using System.Linq;
using System.Windows.Input;
using Regular.Models;
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
            return ruleEditorViewModel.StagingRule.RegexRuleParts.Count > 0;
        }

        public void Execute(object parameter)
        {
            //TODO: Ascertain the rule part being sent to be deleted and remove this 
            ruleEditorViewModel.StagingRule.RegexRuleParts.Remove(ruleEditorViewModel.StagingRule.RegexRuleParts.FirstOrDefault());
            ruleEditorViewModel.CompliantExample = RegexAssembly.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
