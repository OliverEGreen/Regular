using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Regular.Services;
using Regular.Models;
using Regular.Utilities;

namespace Regular
{
    public class RegularApp : IExternalApplication
    {
        public static RegexRuleCacheService RegexRuleCacheService = RegexRuleCacheService.Instance();
        public static DocumentCacheService DocumentCacheService = DocumentCacheService.Instance();
        public static DmUpdaterCacheService DmUpdaterCacheService = DmUpdaterCacheService.Instance();
        
        public static Application RevitApplication { get; set; }
        public Result OnStartup(UIControlledApplication uiControlledApp)
        {
            RegularRibbon.BuildRegularRibbon(uiControlledApp);
            
            // Events which will add to or clean up the AllRegexRules cache
            uiControlledApp.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;
            uiControlledApp.ControlledApplication.DocumentCreated += ControlledApplication_DocumentCreated;
            uiControlledApp.ControlledApplication.DocumentClosing += ControlledApplication_DocumentClosing;
            uiControlledApp.ControlledApplication.DocumentSavedAs += ControlledApplication_DocumentSavedAs;
            return Result.Succeeded;
        }
        private static void ControlledApplication_DocumentSavedAs(object sender, DocumentSavedAsEventArgs e) { RegisterDocument(e.Document); }
        private static void ControlledApplication_DocumentCreated(object sender, DocumentCreatedEventArgs e) { RegisterDocument(e.Document); }
        private static void ControlledApplication_DocumentOpened(object sender, DocumentOpenedEventArgs e) { RegisterDocument(e.Document); }
        private static void ControlledApplication_DocumentClosing(object sender, DocumentClosingEventArgs e){ DeRegisterDocument(e.Document);}
        
        private static void RegisterDocument(Document document)
        {
            // If the document has an existing GUID saved to ExtensibleStorage we retrieve this, otherwise we register it with a new GUID
            string documentGuid = DocumentGuidUtils.GetDocumentGuidFromExtensibleStorage(document) ?? DocumentGuidUtils.RegisterDocumentGuidToExtensibleStorage(document);

            // Creates an accessible, stable reference to the Revit document
            DocumentCacheService.AddDocument(documentGuid, document);

            // Setting a static, accessible reference to the Revit application just this once, for everything to use
            if (RevitApplication == null) RevitApplication = document.Application;

            // NOTE
            // If the document has been opened, saved-as or newly-created, we register the RegularUpdater to it
            // We no longer have one updater, but one per rule. We need to get all rules and register their updaters
            
            //RegularUpdaterUtils.RegisterRegularUpdaterToDocument(documentGuid);
            
            // Getting all of the saved rules in the document
            ObservableCollection<RegexRule> existingRegexRules = ExtensibleStorageUtils.GetAllRegexRulesInExtensibleStorage(documentGuid);
            
            // If there are no saved rules we return, otherwise we establish the updaters
            if (existingRegexRules != null && existingRegexRules.Count < 1) { return; }

            // Rules exist, we need to add the triggers to the RegularUpdater
            DmTriggerUtils.AddAllTriggers(documentGuid, existingRegexRules);
        }
        private static void DeRegisterDocument(Document document)
        {
            string documentGuid = DocumentGuidUtils.GetDocumentGuidFromExtensibleStorage(document) ?? DocumentGuidUtils.RegisterDocumentGuidToExtensibleStorage(document);

            // We can remove all of the triggers
            UpdaterId updaterId = DmUpdaterCacheService.GetUpdater(documentGuid).GetUpdaterId();
            UpdaterRegistry.RemoveDocumentTriggers(updaterId, document);

            // When shutting down the document, we de-register the document-specific RegexRuleUpdater
            RegularUpdaterUtils.DeregisterRegularUpdaterFromDocument(documentGuid);

            // If the RevitDocumentCache contains this document's GUID then we can remove it
            DocumentCacheService.RemoveDocument(documentGuid);
            
            // If there are any saved rules in the application-wide cache, we can remove them
            RegexRuleCacheService.ClearDocumentRules(documentGuid);
        }

        public Result OnShutdown(UIControlledApplication uiControlledApp) { return Result.Succeeded; }
    }
}