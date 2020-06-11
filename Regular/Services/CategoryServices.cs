using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Regular.Services
{
    public static class CategoryServices
    {
        public static Parameter GetParametersOfCategory(Document doc, Category category)
        {
            return null;
        }
        public static Category GetCategoryByName(Document document, string categoryName)
        {
            Categories categoriesList = document.Settings.Categories;
            foreach (Category category in categoriesList) { if (category.Name == categoryName) return category; }
            return null;
        }
        public static List<Category> GetListFromCategorySet(Categories categories)
        {
            List<Category> categoryList = new List<Category>();
            foreach(Category category in categories) { categoryList.Add(category); }
            return categoryList;
        }
        public static CategorySet GetCategorySetFromList(Document doc, List<Category> categories)
        {
            CategorySet categorySet = RegularApp.RevitApplication.Create.NewCategorySet();
            for (int i = 0; i < categories.Count; i++) { categorySet.Insert(categories[i]); }
            return categorySet;
        }
        public static Category GetCategoryFromBuiltInCategory(Document doc, BuiltInCategory builtInCategory)
        {
            Category category = doc.Settings.Categories.get_Item(builtInCategory);
            return category;
        }        
        public static BuiltInCategory GetBuiltInCategoryFromCategory(Category category)
        {
            return (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString());
        }
    }
}
