using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.Models;
using Regular.Services;
using Regular.ViewModels;
using Regular.Utilities;

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
            // Lets the user multi-select and multi-deselect items
            if (!(parameter is CheckBox checkBox)) return;
            ListBox categoriesListBox = WpfUtils.FindParent<ListBox>(checkBox);
            List<CategoryObject> selectedItems = categoriesListBox.SelectedItems.Cast<CategoryObject>().ToList();
            foreach (CategoryObject categoryObject in selectedItems) { categoryObject.IsChecked = checkBox.IsChecked == true; }

            // We update the number of categories ticked
            ruleEditorViewModel.NumberCategoriesSelected = ruleEditorViewModel.
                StagingRule.
                TargetCategoryObjects
                .Count(x => x.IsChecked);
            
            // And need to find out which parameters are now valid for the selection
            ruleEditorViewModel.PossibleTrackingParameterObjects = ParameterServices.
                GetParametersOfCategories(
                    ruleEditorViewModel.DocumentGuid,
                    ruleEditorViewModel.StagingRule.TargetCategoryObjects);

            // Refreshing the list of possible tracking parameter objects
            ParameterObject selectedTrackingParameterObject = ruleEditorViewModel.SelectedTrackingParameterObject;
            
            // If the parameter is still available after categories selection has ended, we don't need to change anything
            if (selectedTrackingParameterObject != null && ruleEditorViewModel.PossibleTrackingParameterObjects.Contains(selectedTrackingParameterObject)) return;
            
            // However if the previously selected tracking parameter is no longer available, we'll default to the first item in the list - or set it to null
            ruleEditorViewModel.SelectedTrackingParameterObject = ruleEditorViewModel.PossibleTrackingParameterObjects.FirstOrDefault();

            // Updating the tracking parameter combobox prompt
            if (ruleEditorViewModel.NumberCategoriesSelected == 0)
            {
                ruleEditorViewModel.ComboBoxTrackingParameterText = "Select Categories";
            }
            else if (ruleEditorViewModel.PossibleTrackingParameterObjects == null || ruleEditorViewModel.PossibleTrackingParameterObjects.Count < 1)
            {
                ruleEditorViewModel.ComboBoxTrackingParameterText = "No Common Parameters Found";
            }
            else
            {
                ruleEditorViewModel.ComboBoxTrackingParameterText = "";
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
