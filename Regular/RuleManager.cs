using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows;
using Regular.Views;
using Regular.Services;

namespace Regular
{
    [Transaction(TransactionMode.Manual)]
    public class RuleManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                string documentGuid = DocumentServices.GetRevitDocumentGuid(commandData.Application.ActiveUIDocument.Document);
                Views.RuleManager ruleManager = new Views.RuleManager(documentGuid);
                ruleManager.ShowDialog();
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

