using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Regular.Services
{
    public static class CategoryServices
    {
        public static List<Category> GetListFromCategorySet(Categories categories)
        {
            List<Category> categoryList = new List<Category>();
            foreach(Category category in categories) { categoryList.Add(category); }
            return categoryList;
        }
        public static CategorySet GetCategorySetFromList(List<Category> categories)
        {
            CategorySet categorySet = RegularApp.RevitApplication.Create.NewCategorySet();
            for (int i = 0; i < categories.Count; i++) { categorySet.Insert(categories[i]); }
            return categorySet;
        }

        public static BuiltInCategory GetBuiltInCategoryFromCategory(Category category)
        {
            return (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString());
        }
    }
}
