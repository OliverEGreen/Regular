using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.UI.SelectElements.ViewModel;
using Regular.Utilities;

namespace Regular.UI.SelectElements.Commands
{
    public class TriggerSelectElementCommand : ICommand
    {
        public SelectElementsViewModel SelectElementsViewModel { get; set; }
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            CheckBox senderCheckBox = (CheckBox) parameter;
            DataGrid dataGrid = VisualTreeUtils.FindParent<DataGrid>(senderCheckBox);
            IList selectedListViewItemCollection = dataGrid.SelectedItems;

            if (selectedListViewItemCollection.Count > 1)
            {
                // Filter down to just the selected items which aren't aligned with the sender checkbox
                List<ObservableObject> selectedObservableObjects = selectedListViewItemCollection
                    .Cast<ObservableObject>()
                    .Where(x => x.IsChecked != senderCheckBox.IsChecked)
                    .ToList();

                foreach (ObservableObject observableObject in selectedObservableObjects)
                {
                    bool originalValue = observableObject.IsChecked;
                    observableObject.IsChecked = !(originalValue);
                }
            }

            SelectElementsViewModel.RefreshSelectedElementCount();
        }

        public TriggerSelectElementCommand(SelectElementsViewModel selectElementsViewModel)
        {
            SelectElementsViewModel = selectElementsViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
