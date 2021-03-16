using System;
using System.Windows.Input;
using Regular.UI.SelectElements.ViewModel;
using CheckBox = System.Windows.Controls.CheckBox;

namespace Regular.UI.SelectElements.Commands
{
    public class TriggerSelectAllElementsCommand : ICommand
    {
        public SelectElementsViewModel  SelectElementsViewModel { get; set; }
        public bool CanExecute(object parameter) => true;
        
        public void Execute(object parameter)
        {
            // Lets the user multi-select and multi-deselect items
            if (!(parameter is CheckBox checkBox)) return;
            foreach (ObservableObject observableObject in SelectElementsViewModel.InputObservableObjects) { observableObject.IsChecked = checkBox.IsChecked == true; }
            SelectElementsViewModel.RefreshSelectedElementCount();
        }

        public TriggerSelectAllElementsCommand(SelectElementsViewModel selectElementsViewModel )
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
