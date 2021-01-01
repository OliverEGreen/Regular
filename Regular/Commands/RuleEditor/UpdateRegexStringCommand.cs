using System;
using System.Windows.Input;
using Regular.Utilities;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class UpdateRegexStringCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public UpdateRegexStringCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter) => true;
        
        public void Execute(object parameter)
        {
            ruleEditorViewModel.StagingRule.RegexString = RegexAssemblyUtils.AssembleRegexString(ruleEditorViewModel.StagingRule);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}