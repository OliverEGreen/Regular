using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace Regular
{
    public class RegularApp : IExternalApplication
    {
       public Result OnStartup(UIControlledApplication uiControlledApp)
        {
            //Creates the Regular Revit Ribbon
            RegularRibbon.BuildRegularRibbon(uiControlledApp);
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiControlledApp) { return Result.Succeeded; }
    }
}