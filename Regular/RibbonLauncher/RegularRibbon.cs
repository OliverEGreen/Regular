using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.UI;
using Regular.RibbonLauncher.Enums;
using Regular.Utilities;

namespace Regular.RibbonLauncher
{
    public class RegularRibbon
    {
        public static UIControlledApplication UiControlledApplication { get; set; }
        public static string AssemblyPath = Assembly.GetExecutingAssembly().Location;
        public static string RegularTabName = "Regular";
        public static Dictionary<string, RibbonPanel> RibbonPanels { get; set; } = new Dictionary<string, RibbonPanel>();
        
        public static Result BuildRegularRibbon(UIControlledApplication uiControlledApp)
        {
            UiControlledApplication = uiControlledApp;
            
            void BuildRibbonTab(string tabName) => UiControlledApplication.CreateRibbonTab(tabName);
            void BuildRibbonPanels()
            {
                void BuildRibbonPanel(string panelName)
                {
                    RibbonPanel ribbonPanel = UiControlledApplication.CreateRibbonPanel(RegularTabName, panelName);
                    RibbonPanels.Add(panelName, ribbonPanel);
                }
                
                List<string> allPanelNames = RegularToolObjects
                    .Select(x => x.RegularToolGroup.GetEnumDescription())
                    .Distinct()
                    .ToList();

                foreach (string panelName in allPanelNames)
                {
                    BuildRibbonPanel(panelName);
                }
            }
            void BuildRibbonButtons()
            {
                void BuildRibbonButton(RegularTool regularTool)
                {
                    string toolGroupFriendlyName = regularTool.RegularToolGroup.GetEnumDescription();
                    RibbonPanel ribbonPanel = RibbonPanels[toolGroupFriendlyName];
                    if (ribbonPanel == null) return;
                    
                    switch (regularTool.RibbonButtonType)
                    {
                        case RibbonButtonType.PushButton:
                            PushButtonData pushButtonData = new PushButtonData(regularTool.InternalName, regularTool.DisplayName, AssemblyPath, regularTool.FullClassName);
                            if (!(ribbonPanel.AddItem(pushButtonData) is PushButton pushButton)) break;
                            pushButton.ToolTip = regularTool.Description;
                            if(regularTool.BitmapImage != null) pushButton.LargeImage = regularTool.BitmapImage;
                            break;
                        case RibbonButtonType.PulldownButtonHeader:
                            PulldownButtonData pulldownButtonData = new PulldownButtonData(regularTool.InternalName, regularTool.DisplayName);
                            if (!(ribbonPanel.AddItem(pulldownButtonData) is PulldownButton pulldownButton)) break;
                            pulldownButton.ToolTip = regularTool.Description;
                            if(regularTool.BitmapImage != null) pulldownButton.LargeImage = regularTool.BitmapImage;
                            string condensedTopButtonName = StringUtils.StripToAlphanumeric(regularTool.InternalName);
                            foreach (RegularTool dropdownRegularTool in regularTool.DropdownButtonData)
                            {
                                string condensedDropdownButtonName = StringUtils.StripToAlphanumeric(dropdownRegularTool.InternalName);
                                string className = $"Regular.Tools.{condensedTopButtonName}.{condensedDropdownButtonName}";
                                PushButtonData dropdownPushButtonData = new PushButtonData
                                (
                                    dropdownRegularTool.InternalName,
                                    dropdownRegularTool.DisplayName,
                                    AssemblyPath,
                                    className
                                );
                                PushButton dropdownPushButton = pulldownButton.AddPushButton(dropdownPushButtonData);
                                if(dropdownRegularTool.BitmapImage != null) dropdownPushButton.LargeImage = dropdownRegularTool.BitmapImage;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                // Looping through all defined buttons, creating them and adding them to their respective panels
                foreach (RegularTool regularTool in RegularToolObjects) BuildRibbonButton(regularTool);
            }
            
            BuildRibbonTab(RegularTabName);
            BuildRibbonPanels();
            BuildRibbonButtons();
            return Result.Succeeded;
        }
        
        public static List<RegularTool> RegularToolObjects = new List<RegularTool>
        {
            new RegularTool(RegularToolGroup.DataSpec, RibbonButtonType.PushButton, "Rule Manager", "Description goes here!"),
            new RegularTool(RegularToolGroup.DataSpec, RibbonButtonType.PulldownButtonHeader, "Transfer Rules", "Description goes here!")
            {
                DropdownButtonData = new List<RegularTool>
                {
                    new RegularTool(RegularToolGroup.DataSpec, RibbonButtonType.PulldownButtonMenuItem, "Export Rules", "Export your Regular rules to JSON format to transfer them between documents."),
                    new RegularTool(RegularToolGroup.DataSpec, RibbonButtonType.PulldownButtonMenuItem, "Import Rules", "Import one of more Regular rules from a JSON file."),
                }
            }
        };
    }
}