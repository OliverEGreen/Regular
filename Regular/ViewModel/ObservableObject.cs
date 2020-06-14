using Autodesk.Revit.DB;
using Regular.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Regular.ViewModel
{
    public class ObservableObject : INotifyPropertyChanged
    {
        // Using these to bind categories to a checkbox list
        // Ids get stored in ExtensibleStorage, Names are displayed to the user 
        // and IsChecked is used to save the checkbox state for each object
        
        private string name;
        private string id;
        private bool isChecked;
                        
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public string Id
        {
            get => id;
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        public static ObservableCollection<ObservableObject> GetInitialCategories(string documentGuid)
        {
            ObservableCollection<ObservableObject> observableObjects = new ObservableCollection<ObservableObject>();
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);

            // Fetching all categories to create ObservableObjects
            List<Category> userVisibleCategories = CategoryServices.GetListFromCategorySet(document.Settings.Categories)
                .Where(x => x.AllowsBoundParameters)
                .OrderBy(x => x.Name)
                .ToList();
            
            foreach(Category category in userVisibleCategories)
            {
                observableObjects.Add(new ObservableObject() { Name = category.Name, Id = category.Id.ToString(), isChecked = false });
            }
            return observableObjects;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

