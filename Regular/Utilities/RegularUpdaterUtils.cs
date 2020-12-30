using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Regular.Utilities
{
    public static class RegularUpdaterUtils
    {
        public static void RegisterRegularUpdaterToDocument(string documentGuid)
        {
            // We register the RegularUpdater to each document as it opens
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            RegularUpdater regularUpdater = new RegularUpdater(RegularApp.RevitApplication.ActiveAddInId);
            RegularApp.DmUpdaterCacheService.AddUpdater(documentGuid, regularUpdater);
            
            // Using the optional boolean flag so the updater doesn't pop up with a massive scary message on loading
            try { UpdaterRegistry.RegisterUpdater(regularUpdater, document, true); }
            catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }
        }
        public static void DeregisterRegularUpdaterFromDocument(string documentGuid)
        {
            // We deregister the RegularUpdater from each document as it closes
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            // Attempting to deregister the RegularUpdater
            try { UpdaterRegistry.UnregisterUpdater(RegularApp.DmUpdaterCacheService.GetUpdater(documentGuid).GetUpdaterId(), document); }
            catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }

            RegularApp.DmUpdaterCacheService.RemoveUpdater(documentGuid);
        }
    }
}
