using System;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Regular.Utilities;
using Regular.Views;

namespace Regular
{
    [Transaction(TransactionMode.Manual)]
    public class LaunchRuleManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                if (RegularApp.DialogShowing) return Result.Cancelled;
                string documentGuid = DocumentGuidUtils.GetDocumentGuidFromExtensibleStorage(commandData.Application.ActiveUIDocument.Document);
                RuleManagerView ruleManagerView = new RuleManagerView(documentGuid);
                ruleManagerView.ShowDialog();
                RegularApp.DialogShowing = false;
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

