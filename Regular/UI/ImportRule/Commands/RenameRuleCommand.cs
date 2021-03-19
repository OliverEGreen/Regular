using System;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.ImportRule.ViewModel;

namespace Regular.UI.ImportRule.Commands
{
    public class RenameRuleCommand : ICommand
    {
    public ImportRuleViewModel ImportRuleViewModel { get; }
    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
        RegexRule.SaveRenamedRegexRule
        (
            ImportRuleViewModel.ImportRuleInfo.DocumentGuid,
            ImportRuleViewModel.ImportRuleInfo.NewRegexRule
        );
    }

    public RenameRuleCommand(ImportRuleViewModel importRuleViewModel)
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