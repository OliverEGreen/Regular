using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class TriggerSelectAllCategoriesCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public TriggerSelectAllCategoriesCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter) => ruleEditorViewModel.CategoriesPanelExpanded;
        
        public void Execute(object parameter)
        {
            // Lets the user multi-select and multi-deselect items
            if (!(parameter is CheckBox checkBox)) return;
            foreach (CategoryObject categoryObject in ruleEditorViewModel.StagingRule.TargetCategoryObjects) { categoryObject.IsChecked = checkBox.IsChecked == true; }
            
            // We update the number of categories ticked
            ruleEditorViewModel.UpdateCheckedCategoriesCount();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}