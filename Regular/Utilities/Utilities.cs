using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public class Utilities
    {
        //Spiderinnet helper method
        public static List<T> RawConvertSetToList<T>(IEnumerable set)
        {
            List<T> list = (from T p in set select p).ToList<T>();
            return list;
        }

        public static Category FetchCategoryByName(string categoryName, Document doc)
        {
            Categories categoriesList = doc.Settings.Categories;
            foreach (Category category in categoriesList)
            {
                if (category.Name == categoryName) { return category; }
            }
            return null;
        }

        //Spiderinnet's hacky method to create a project parameter, despite the Revit API's limitations on this
        //From https://spiderinnet.typepad.com/blog/2011/05/parameter-of-revit-api-31-create-project-parameter.html
        //This creates a temporary shared parameters file, a temporary shared parameter
        //It then binds this back to the model as an InstanceBinding and deletes the temporary stuff
        public static void CreateProjectParameter(Document doc, Application app, string parameterName, ParameterType parameterType, CategorySet categorySet, BuiltInParameterGroup builtInParameterGroup, bool isInstanceParameter)
        {
            Transaction transaction = new Transaction(doc, "Regular Project Parameters Test");
            transaction.Start();
            
            string oriFile = app.SharedParametersFilename;
            string tempFile = Path.GetTempFileName() + ".txt";
            using (File.Create(tempFile)) { }
            app.SharedParametersFilename = tempFile;
            ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterName, parameterType);
            ExternalDefinition def = app.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;

            app.SharedParametersFilename = oriFile;
            File.Delete(tempFile);

            Binding binding = app.Create.NewTypeBinding(categorySet);
            if (isInstanceParameter) binding = app.Create.NewInstanceBinding(categorySet);

            BindingMap bindingMap = (new UIApplication(app)).ActiveUIDocument.Document.ParameterBindings;
            bindingMap.Insert(def, binding, builtInParameterGroup);

            transaction.Commit();
        }

        public static Category GetCategoryFromBuiltInCategory(Document doc, BuiltInCategory builtInCategory)
        {
            Category category = doc.Settings.Categories.get_Item(builtInCategory);
            if (category != null) return category;
            return null;
        }

        public static CategorySet CreateCategorySetFromListOfCategories(Document doc, Application app, List<Category> categories)
        {
            CategorySet categorySet = app.Create.NewCategorySet();
            for(int i = 0; i < categories.Count; i++) { categorySet.Insert(categories[i]); }
            return categorySet;
        }

    }
}
