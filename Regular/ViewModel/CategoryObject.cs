using Autodesk.Revit.DB;
using Regular.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Regular.ViewModel
{
    public class CategoryObject : INotifyPropertyChanged
    {
        // Using these to bind categories to a checkbox list
        // Ids get stored in ExtensibleStorage, Names are displayed to the user 
        // and IsChecked is used to save the checkbox state for each object
        
        private string categoryObjectName;
        private int categoryObjectId;
        private bool isChecked;
                        
        public string CategoryObjectName
        {
            get => categoryObjectName;
            set
            {
                categoryObjectName = value;
                NotifyPropertyChanged("RuleName");
            }
        }
        public int CategoryObjectId
        {
            get => categoryObjectId;
            set
            {
                categoryObjectId = value;
                NotifyPropertyChanged("CategoryObjectId");
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

        public static ObservableCollection<CategoryObject> GetInitialCategories(string documentGuid)
        {
            ObservableCollection<CategoryObject> observableObjects = new ObservableCollection<CategoryObject>();
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);

            // Fetching all categories to create ObservableObjects
            List<Category> userVisibleCategories = CategoryServices.GetListFromCategorySet(document.Settings.Categories)
                .Where(x => x.AllowsBoundParameters)
                .OrderBy(x => x.Name)
                .ToList();
            
            foreach(Category category in userVisibleCategories)
            {
                observableObjects.Add(new CategoryObject() { CategoryObjectName = category.Name, CategoryObjectId = category.Id.IntegerValue, isChecked = false });
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

