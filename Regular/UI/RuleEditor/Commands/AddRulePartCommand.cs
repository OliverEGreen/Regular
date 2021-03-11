using System;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.RuleEditor.ViewModel;

namespace Regular.UI.RuleEditor.Commands
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
            ruleEditorViewModel.GenerateCompliantExampleCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
