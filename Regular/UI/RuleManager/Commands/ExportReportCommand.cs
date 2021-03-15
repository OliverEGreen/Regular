using System;
using System.Windows.Input;
using Regular.UI.RuleManager.ViewModel;

namespace Regular.UI.RuleManager.Commands
{
    public class ExportReportCommand : ICommand
    {
        private readonly RuleManagerViewModel ruleManagerViewModel;

        public ExportReportCommand(RuleManagerViewModel ruleManagerViewModel)
        {
            this.ruleManagerViewModel = ruleManagerViewModel;
        }

        public bool CanExecute(object parameter) => ruleManagerViewModel.ExportReportEnabled;
        
        public void Execute(object parameter)
        {
            
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}