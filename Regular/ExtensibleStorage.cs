using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System.Windows;

[Transaction(TransactionMode.Manual)]
public class ExtensibleStorage : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiapp = commandData.Application;
        Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        try
        {
            // Since there were no schemas we'll load up the RuleManager with an empty list.
            // Now we've got the schema, we build the DataStorage object
            using (Transaction transaction = new Transaction(doc, "Regular Building Schema"))
            {
                // Creating document-wide extensible storage
                transaction.Start();
                DataStorage regularDataStorage = DataStorage.Create(doc);
                transaction.Commit();
            }

            // Building an Entity of the Schema we defined. This needs to happen later on when the user
            // Is interacting with the rulemanager form, adding in new rules and all that stuff.
            // Entity regularEntity = new Entity(regularSchema);

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return Result.Failed;
        }
    }
}