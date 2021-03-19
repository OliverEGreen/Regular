using System;
using System.Windows.Input;
using Regular.UI.ImportRule.Enums;
using Regular.UI.ImportRule.ViewModel;

namespace Regular.UI.ImportRule.Commands
{
    public class RenameAllCommand : ICommand
    {
        public ImportRuleViewModel ImportRuleViewModel {get;}
        public bool CanExecute(object parameter) => true;
        
        public void Execute(object parameter)
        {
            ImportRuleViewModel.OverrideMode = OverrideMode.RenameAll;
            ImportRuleViewModel.RenameRuleCommand.Execute(null);
        }

        public RenameAllCommand(ImportRuleViewModel importRuleViewModel)
        {
            ImportRuleViewModel = importRuleViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}