using System;
using System.Linq;
using System.Windows.Input;
using Regular.UI.SelectElements.ViewModel;

namespace Regular.UI.SelectElements.Commands
{
    public class SelectElementsCommand : ICommand
    {
        public SelectElementsViewModel SelectElementsViewModel { get; set; }

        public bool CanExecute(object parameter)
        {
            return SelectElementsViewModel.InputObservableObjects.Count(x => x.IsChecked) > 0;
        }

        public void Execute(object parameter) { }

        public SelectElementsCommand(SelectElementsViewModel selectElementsViewModel)
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
