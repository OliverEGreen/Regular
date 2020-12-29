using System;
using System.Windows;
using System.Windows.Input;
using Regular.Utilities;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
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
            ruleEditorViewModel.CompliantExample = RegexAssemblyUtils.GenerateRandomExample(ruleEditorViewModel.StagingRule.RegexRuleParts);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
