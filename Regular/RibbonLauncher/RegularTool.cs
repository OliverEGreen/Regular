using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Regular.RibbonLauncher.Enums;
using Regular.Utilities;

namespace Regular.RibbonLauncher
{
    public class RegularTool
    {
        public string ToolGuid { get; }
        public string InternalName { get; }
        public string DisplayName { get; set; }
        public string FullClassName { get; }
        public string ClassName { get; }
        public string Description { get; set; }
        public BitmapImage BitmapImage { get; set; }
        public RegularToolGroup RegularToolGroup { get; set; }
        public RibbonButtonType RibbonButtonType { get; set; }
        public List<RegularTool> DropdownButtonData { get; set; } = null;
        
        
        public RegularTool(RegularToolGroup regularToolGroup, RibbonButtonType ribbonButtonType, string toolName, string description)
        {
            ToolGuid = Guid.NewGuid().ToString();
            RegularToolGroup = regularToolGroup;
            RibbonButtonType = ribbonButtonType;
            InternalName = toolName;
            
            // Button tooltip information
            Description = description;
            
            // Splits the name across multiple lines to best-fit icons to the ribbon
            DisplayName = StringUtils.WrapString(InternalName, 15);
            
            // We automatically strip out the description to get the class name, ensuring they align
            string condensedName = Regex.Replace(InternalName, "[^A-Za-z0-9]", "");
            
            ClassName = condensedName;

            // The class name and its namespace folder must match exactly
            FullClassName = $"Regular.Tools.{ClassName}.{ClassName}";
            
            // The class name and the PNG icon name must match exactly
            FileInfo fileInfo = new FileInfo($@"C:\Users\ogreen\Source\Repos\Regular\Regular\Resources\{condensedName}.png");
            if (!fileInfo.Exists) return;
            
            // If the .png can be found, we instantiate it for the button creation
            BitmapImage = new BitmapImage(new Uri($"pack://application:,,,/Regular;component/Resources/{condensedName}.png"));
        }
    }
}