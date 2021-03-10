using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace Regular.Utilities
{
    public class RegularRibbon
    {
        public static Result BuildRegularRibbon(UIControlledApplication uiControlledApp)
        {
            const string tabName = "Regular";
            
            uiControlledApp.CreateRibbonTab(tabName);
            RibbonPanel ribbonPanelDataSpec = uiControlledApp.CreateRibbonPanel(tabName, "Data Spec");
            RibbonPanel ribbonPanelDataPalette = uiControlledApp.CreateRibbonPanel(tabName, "Data Palette");
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData ruleManagerButtonData = new PushButtonData("Rule Manager", "Rule" + Environment.NewLine + "Manager", thisAssemblyPath, "Regular.LaunchRuleManager");
            PushButton ruleManagerButton = ribbonPanelDataSpec.AddItem(ruleManagerButtonData) as PushButton;
            ruleManagerButton.ToolTip = "Build simple data validation rules to test against parts of your model.";
            BitmapImage ruleManagerButtonIcon = new BitmapImage(new Uri("pack:// application:,,,/Regular;component/Resources/RegularIcon32px.png"));
            ruleManagerButton.LargeImage = ruleManagerButtonIcon;

            PushButtonData exportRulesButtonData = new PushButtonData("Export Rules", "Export" + Environment.NewLine + "Rules", thisAssemblyPath, "Regular.LaunchRuleManager");
            PushButton exportRulesButton = ribbonPanelDataSpec.AddItem(exportRulesButtonData) as PushButton;
            exportRulesButton.ToolTip = "Export your rules to JSON format for re-use in other projects.";
            BitmapImage exportRulesButtonIcon = new BitmapImage(new Uri("pack:// application:,,,/Regular;component/Resources/ExportIcon32px.png"));
            exportRulesButton.LargeImage = exportRulesButtonIcon;

            return Result.Succeeded;
        }
    }
}
