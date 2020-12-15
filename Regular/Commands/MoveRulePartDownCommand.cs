using System;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Commands
{
    public class MoveRulePartDownCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public MoveRulePartDownCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            //TODO: Need to validate whether we can move the rule part down, check index isn't last in the list
            return true;
            ListBox rulePartsListBox = parameter as ListBox;
            RegexRulePart regexRulePart =  rulePartsListBox.SelectedItem as RegexRulePart;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(regexRulePart);
            return index >= ruleEditorViewModel.StagingRule.RegexRuleParts.Count;
        }

        public void Execute(object parameter)
        {
            ListBox rulePartsListBox = parameter as ListBox;
            RegexRulePart regexRulePart = rulePartsListBox.SelectedItem as RegexRulePart;
            int index = ruleEditorViewModel.StagingRule.RegexRuleParts.IndexOf(regexRulePart);

            ruleEditorViewModel.StagingRule.RegexRuleParts.RemoveAt(index);
            ruleEditorViewModel.StagingRule.RegexRuleParts.Insert(index + 1, regexRulePart);
            //ListBoxRuleParts.Focus();
            //ListBoxRuleParts.SelectedItem = regexRulePart;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
