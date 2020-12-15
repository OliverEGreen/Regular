using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Regular.Models;
using static Regular.RegularApp;

namespace Regular.Services
{
    public static class DmUpdaterServices
    {
        
        public static void RegisterRegularUpdaterToDocument(string documentGuid)
        {
            // We register the RegularUpdater to each document as it opens
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
            
            RegularUpdater regularUpdater = new RegularUpdater(RevitApplication.ActiveAddInId);
            DmUpdaters.AllUpdaters[documentGuid] = regularUpdater;
            // Using the optional boolean flag so the updater doesn't pop up with a massive scary message on loading
            try { UpdaterRegistry.RegisterUpdater(regularUpdater, document, true); }
            catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }
        }
        public static void DeregisterRegularUpdaterFromDocument(string documentGuid)
        {
            // We deregister the RegularUpdater from each document as it closes
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
            
            // Attempting to deregister the RegularUpdater
            try { UpdaterRegistry.UnregisterUpdater(DmUpdaters.AllUpdaters[documentGuid].GetUpdaterId(), document); }
            catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }

            DmUpdaters.AllUpdaters.Remove(documentGuid);
        }
    }
}
