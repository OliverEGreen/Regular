using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Regular.ViewModel;
using Regular.Services;
using Regular.Model;

namespace Regular
{
    public class RegularApp : IExternalApplication
    {
        public static Dictionary<string, Document> RevitDocumentCache { get; set; }
        public static Application RevitApplication { get; set; }
        public Result OnStartup(UIControlledApplication uiControlledApp)
        {
            RegularRibbon.BuildRegularRibbon(uiControlledApp);
            
            // This keeps track of sets of rules per document, so is initialized as soon as possible
            RegexRules.AllRegexRules = new Dictionary<string, ObservableCollection<RegexRule>>();
            RevitDocumentCache = new Dictionary<string, Document>();
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
        private static void RegisterDocument(Document document)
        {
            // If the document has an existing GUID saved to ExtensibleStorage we retrieve this, otherwise we register it with a new GUID
            string documentGuid = ExtensibleStorageServices.GetDocumentGuidFromExtensibleStorage(document) ?? ExtensibleStorageServices.RegisterDocumentGuidToExtensibleStorage(document);

            // Setting a static, accessible reference to the Revit application just this once, for everything to use.
            if(RevitApplication == null) RevitApplication = document.Application;

            // Creates an accessible, stable reference to the Revit document
            RevitDocumentCache[documentGuid] = document;
            ObservableCollection<RegexRule> existingRegexRules = ExtensibleStorageServices.GetAllRegexRulesInExtensibleStorage(documentGuid);
            RegexRules.AllRegexRules[documentGuid] = existingRegexRules ?? new ObservableCollection<RegexRule>();

            // If there are no saved rules we return, otherwise we establish the updaters
            if (existingRegexRules != null && existingRegexRules.Count < 1) { return; }

            if (existingRegexRules == null) return;
            foreach (RegexRule regexRule in existingRegexRules)
            {
                DynamicModelUpdateServices.RegisterRegexRule(documentGuid, regexRule.RuleGuid);
            }

            //TaskDialog.Show("Regular", $"{existingRegexRules.Count} rules registered to DMU upon opening.");
        }
        private static void ControlledApplication_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            Document document = e.Document;
            string documentId = DocumentServices.GetRevitDocumentGuid(document);
            
            // Cleaning up the Revit Document and AllRegexRules Caches
            if (RevitDocumentCache.ContainsKey(documentId)) { RevitDocumentCache.Remove(documentId); }
            if (RegexRules.AllRegexRules.ContainsKey(documentId)) { RegexRules.AllRegexRules.Remove(documentId); }
        }
        public Result OnShutdown(UIControlledApplication uiControlledApp) { return Result.Succeeded; }
    }
}