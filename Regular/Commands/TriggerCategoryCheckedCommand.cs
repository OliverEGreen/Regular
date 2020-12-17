using System;
using System.Linq;
using System.Windows.Input;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;

namespace Regular.Commands
{
    public class TriggerCategoryCheckedCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public TriggerCategoryCheckedCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }

        public bool CanExecute(object parameter) => ruleEditorViewModel.CategoriesPanelExpanded;

        public void Execute(object parameter)
        {
            ParameterObject selectedTrackingParameterObject = ruleEditorViewModel.SelectedTrackingParameterObject;
            
            // We update the target category objects
            ruleEditorViewModel.NumberCategoriesSelected = ruleEditorViewModel.
                StagingRule.
                TargetCategoryObjects
                .Count(x => x.IsChecked);
            // And need to find out which parameters are now valid for the selection
            ruleEditorViewModel.PossibleTrackingParameterObjects = ParameterServices.
                GetParametersOfCategories(
                    ruleEditorViewModel.DocumentGuid,
                    ruleEditorViewModel.StagingRule.TargetCategoryObjects);

            if (selectedTrackingParameterObject != null && ruleEditorViewModel.PossibleTrackingParameterObjects.Contains(selectedTrackingParameterObject)) return;
            ruleEditorViewModel.SelectedTrackingParameterObject = ruleEditorViewModel.PossibleTrackingParameterObjects.FirstOrDefault();

            // Updating the tracking parameter combobox prompt
            if (ruleEditorViewModel.NumberCategoriesSelected == 0)
            {
                ruleEditorViewModel.ComboBoxTrackingParameterText = "Select Categories";
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
