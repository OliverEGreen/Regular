using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace Regular
{
    public class RegularRibbon
    {
        public static Result BuildRegularRibbon(UIControlledApplication app)
        {
            const string tabName = "Regular";
            app.CreateRibbonTab(tabName);
            var ribbonPanelGeneral = app.CreateRibbonPanel(tabName, "Regular");
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData ruleManagerButtonData = new PushButtonData("Rule Manager", "Rule" + Environment.NewLine + "Manager", thisAssemblyPath, "Regular.RuleManager");
            PushButton ruleManagerButton = ribbonPanelGeneral.AddItem(ruleManagerButtonData) as PushButton;
            ruleManagerButton.ToolTip = "Build simple data validation rules to test against parts of your model.";

            return Result.Succeeded;
        }
    }
}
