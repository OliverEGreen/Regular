using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows;


namespace Regular
{
    [Transaction(TransactionMode.Manual)]
    public class RuleManager : IExternalCommand
    {
        public static Document Document { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
                        
            try
            {
                Views.RuleManager ruleManager = new Views.RuleManager(doc);
                ruleManager.ShowDialog();
                // We need to build the rule manager UI using IObservableCollection and Listbox. 
                // Need to build the new rule button in order to have ability to create new rules
                // This will involve building a parsing function to parse a new, sort of empty RegexRule into ExtensibleStorage
                // This will involve Entity and DataStorage object stuff, the reverse of before.

                // When the dialog closes we can run all rules in order
                // After rules have run this External Command is over.
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }
    }

}

