using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.Models;
using Regular.UI.RuleEditor.ViewModel;
using Regular.Utilities;

namespace Regular.UI.RuleEditor.Commands
{
    public class TriggerSelectCategoryCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public TriggerSelectCategoryCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }

        public bool CanExecute(object parameter) => ruleEditorViewModel.CategoriesPanelExpanded;

        public void Execute(object parameter)
        {
            // Lets the user multi-select and multi-deselect items
            if (!(parameter is CheckBox checkBox)) return;
            DataGrid categoriesDataGrid = VisualTreeUtils.FindParent<DataGrid>(checkBox);
            List<CategoryObject> selectedItems = categoriesDataGrid.SelectedItems.Cast<CategoryObject>().ToList();

            if (selectedItems.Count > 1)
            {
                foreach (CategoryObject categoryObject in selectedItems) categoryObject.IsChecked = checkBox.IsChecked == true;
            }

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
