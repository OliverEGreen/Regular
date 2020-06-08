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
        private string name;
        private string id;
        private bool isChecked;
                        
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }


        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        public static ObservableCollection<ObservableObject> GetInitialCategories(Document document)
        {
            ObservableCollection<ObservableObject> observableObjects = new ObservableCollection<ObservableObject>();
            // We have to pass this to the regexrule as it's created somehow.
            List<Category> userVisibleCategories = CategoryServices.GetListFromCategorySet(document.Settings.Categories).Where(x => x.AllowsBoundParameters == true).OfType<Category>().ToList();
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

