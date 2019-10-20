using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public class Utilities
    {
        public static Category FetchCategoryByName(string categoryName, Document doc)
        {
            Categories categoriesList = doc.Settings.Categories;
            foreach (Category category in categoriesList)
            {
                if (category.Name == categoryName) { return category; }
            }
            return null;
        }
    }
}
