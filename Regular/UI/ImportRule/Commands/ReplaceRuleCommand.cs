using System;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.ImportRule.ViewModel;

namespace Regular.UI.ImportRule.Commands
{
    public class ReplaceRuleCommand : ICommand
    {
        public ImportRuleViewModel ImportRuleViewModel {get;}
        public bool CanExecute(object parameter) => true;
        
        public void Execute(object parameter)
        {
            RegexRule.ReplaceRegexRule
            (
                ImportRuleViewModel.ImportRuleInfo.DocumentGuid, 
                ImportRuleViewModel.ImportRuleInfo.ExistingRegexRule, 
                ImportRuleViewModel.ImportRuleInfo.NewRegexRule 
            );
        }

        public ReplaceRuleCommand(ImportRuleViewModel importRuleViewModel)
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