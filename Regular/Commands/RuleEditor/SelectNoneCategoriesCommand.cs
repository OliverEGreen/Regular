using System;
using System.Linq;
using System.Windows.Input;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class SelectNoneCategoriesCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public SelectNoneCategoriesCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter) => ruleEditorViewModel.CategoriesPanelExpanded;

        public void Execute(object parameter)
        {
            // We tick all of the categories on
            for (int i = 0; i < ruleEditorViewModel.StagingRule.TargetCategoryObjects.Count; i++)
            {
                ruleEditorViewModel.StagingRule.TargetCategoryObjects[i].IsChecked = false;
            }

            // We update the number of categories ticked
            ruleEditorViewModel.NumberCategoriesSelected = ruleEditorViewModel.
                StagingRule.
                TargetCategoryObjects
                .Count(x => x.IsChecked);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}