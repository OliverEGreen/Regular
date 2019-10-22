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
            BitmapImage ruleManagerButtonIcon = new BitmapImage(new Uri("pack://application:,,,/Regular;component/Resources/RegularIcon32px.png"));
            ruleManagerButton.LargeImage = ruleManagerButtonIcon;

            PushButtonData exportRulesButtonData = new PushButtonData("Export Rules", "Export" + Environment.NewLine + "Rules", thisAssemblyPath, "Regular.RuleManager");
            PushButton exportRulesButton = ribbonPanelGeneral.AddItem(exportRulesButtonData) as PushButton;
            exportRulesButton.ToolTip = "Export your rules to JSON format for re-use in other projects.";
            BitmapImage exportRulesButtonIcon = new BitmapImage(new Uri("pack://application:,,,/Regular;component/Resources/ExportIcon32px.png"));
            exportRulesButton.LargeImage = exportRulesButtonIcon;

            return Result.Succeeded;
        }
    }
}
