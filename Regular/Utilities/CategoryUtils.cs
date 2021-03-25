using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Regular.Models;

namespace Regular.Utilities
{
    public static class CategoryUtils
    {
        public static List<Category> ConvertCategorySetToList(Categories categories)
        {
            return categories.Cast<Category>().ToList();
        }
        
        public static BuiltInCategory GetBuiltInCategoryFromCategory(Category category)
        {
            return (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString());
        }

        public static ObservableCollection<CategoryObject> GetInitialCategories(string documentGuid)
        {
            // Relevant categories are assigned by the user using a Checkbox List
            // CategoryObjects tie together the category name, its ID and its checked state as a Boolean
            // Here we return all possible category objects for the new RegexRule and set them all to unchecked

            ObservableCollection<CategoryObject> observableObjects = new ObservableCollection<CategoryObject>();
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            // Fetching all categories to create ObservableObjects
            List<Category> userVisibleCategories = ConvertCategorySetToList(document.Settings.Categories)
                .Where(x => x.AllowsBoundParameters)
                .OrderBy(x => x.Name)
                .ToList();

            foreach (Category category in userVisibleCategories)
            {
                observableObjects.Add(new CategoryObject
                {
                    CategoryObjectName = category.Name,
                    CategoryObjectId = category.Id.IntegerValue,
                    IsChecked = false
                });
            }
            return observableObjects;
        }
    }
}
