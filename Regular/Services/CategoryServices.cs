using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Services
{
    public static class CategoryServices
    {
        public static Parameter GetParametersOfCategory(Document doc, Category category)
        {
            return null;
        }

        public static Category GetCategoryByName(string categoryName, Document doc)
        {
            Categories categoriesList = doc.Settings.Categories;
            foreach (Category category in categoriesList) { if (category.Name == categoryName) return category; }
            return null;
        }

        public static Category GetCategoryFromBuiltInCategory(Document doc, BuiltInCategory builtInCategory)
        {
            Category category = doc.Settings.Categories.get_Item(builtInCategory);
            if (category != null) return category;
            return null;
        }

        public static CategorySet CreateCategorySetFromListOfCategories(Document doc, List<Category> categories)
        {
            CategorySet categorySet = RegularApp.RevitApplication.Create.NewCategorySet();
            for (int i = 0; i < categories.Count; i++) { categorySet.Insert(categories[i]); }
            return categorySet;
        }
    }
}
