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
                string documentGuid = DocumentGuidUtils.GetDocumentGuidFromExtensibleStorage(commandData.Application.ActiveUIDocument.Document);
                RuleManager ruleManager = new RuleManager(documentGuid);
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

