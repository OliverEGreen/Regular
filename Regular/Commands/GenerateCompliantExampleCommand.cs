using System;
using System.Windows;
using System.Windows.Input;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Commands
{
    public class GenerateCompliantExampleCommand: ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public GenerateCompliantExampleCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            return ruleEditorViewModel.StagingRule.RegexRuleParts.Count >= 0;
        }

        public void Execute(object parameter)
        {
            ruleEditorViewModel.CompliantExampleVisibility= Visibility.Visible;
            ruleEditorViewModel.CompliantExample = RegexAssembly.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
