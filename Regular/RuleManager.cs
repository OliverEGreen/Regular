using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows;

[Transaction(TransactionMode.Manual)]
public class RuleManager : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            //The first thing the rule manager needs to do is check the model's ExtensibleStorage
            //We need to see if there are already rules saved in the document.
            //If there are, we load them (they are saved by their GUIDs)
            //If not, we'll begin by defining a RegexRuleApp schema
            TaskDialog.Show("Test", "Rule Manager works!");
            //The Rule Manager is a modal WPF Window. When the dialog closes we can run all rules in order
            //After rules have run this External Command is over.
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return Result.Failed;
        }
    }
}
