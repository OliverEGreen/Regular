using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System.Collections.ObjectModel;
using Regular.Models;
using Regular.Services;

namespace Regular
{
    public class RegularApp : IExternalApplication
    {
        public static Dictionary<string, ObservableCollection<RegexRule>> AllRegexRules { get; set; }
        public static Dictionary<string, Document> RevitDocumentCache { get; set; }
        public static Application RevitApplication { get; set; }
        public Result OnStartup(UIControlledApplication uiControlledApp)
        {
            RegularRibbon.BuildRegularRibbon(uiControlledApp);
            
            // This keeps track of sets of rules per document, so is initialized as soon as possible
            AllRegexRules = new Dictionary<string, ObservableCollection<RegexRule>>();
            RevitDocumentCache = new Dictionary<string, Document>();
            // Events which will add to or clean up the AllRegexRules cache
            uiControlledApp.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;
            uiControlledApp.ControlledApplication.DocumentCreated += ControlledApplication_DocumentCreated;
            uiControlledApp.ControlledApplication.DocumentClosing += ControlledApplication_DocumentClosing;
            uiControlledApp.ControlledApplication.DocumentSavedAs += ControlledApplication_DocumentSavedAs;
            return Result.Succeeded;
        }

        private void ControlledApplication_DocumentSavedAs(object sender, DocumentSavedAsEventArgs e) { RegisterDocument(e.Document); }
        private void ControlledApplication_DocumentCreated(object sender, DocumentCreatedEventArgs e) { RegisterDocument(e.Document); }
        private void ControlledApplication_DocumentOpened(object sender, DocumentOpenedEventArgs e) { RegisterDocument(e.Document); }
        private void RegisterDocument(Document document)
        {
            // If the document has an existing GUID saved to ExtensibleStorage we retrieve this, otherwise we register it with a new GUID
            string documentId = ExtensibleStorageServices.GetDocumentGuidFromExtensibleStorage(document);
            if (documentId == null) documentId = ExtensibleStorageServices.RegisterDocumentGuidToExtensibleStorage(document);

            // Setting a static, accessible reference to the Revit application just this once, for everything to use.
            if(RevitApplication == null) RevitApplication = document.Application;

            // Creates an accessible, stable reference to the Revit document
            RevitDocumentCache[documentId] = document;
            ObservableCollection<RegexRule> existingRegexRules = ExtensibleStorageServices.GetAllRegexRulesInExtensibleStorage(document);
            AllRegexRules[documentId] = existingRegexRules == null ? new ObservableCollection<RegexRule>() : existingRegexRules;

            // If there are no saved rules we return, otherwise we establish the updaters
            if (existingRegexRules != null && existingRegexRules.Count < 1) { return; }

            /*
            foreach (RegexRule regexRule in AllRegexRules[documentId])
            {
                RuleUpdater ruleUpdater = new RuleUpdater(document.Application.ActiveAddInId);
                try
                {
                    UpdaterRegistry.RegisterUpdater(ruleUpdater, document);
                }
                catch(Exception ex)
                {
                    TaskDialog.Show("Test", ex.Message);
                }
                
                
                Category category = regexRule.TargetCategory;
                BuiltInCategory builtInCategory = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString());
                ElementCategoryFilter elementCategoryFilter = new ElementCategoryFilter(builtInCategory);
                // A bit of a fudge - can't figure out how to get the Mark parameter without an element yet...
                Element element = new FilteredElementCollector(document).WherePasses(elementCategoryFilter).WhereElementIsNotElementType().ToElements().ToList().FirstOrDefault();
                if (element == null) return;
                Parameter targetParameter = element.LookupParameter(regexRule.TrackingParameterName);
                if (targetParameter == null) return;
                // Parameter targetParameter = Utilities.FetchProjectParameterByName(_document, regexRule.TrackingParameterName);
                UpdaterRegistry.AddTrigger(ruleUpdater.GetUpdaterId(), elementCategoryFilter, Element.GetChangeTypeParameter(targetParameter.Id));
                List<Element> elements = new FilteredElementCollector(document).OfCategory(builtInCategory).ToElements().ToList();
                TaskDialog.Show("Test", $"Found {elements.Count} elements of category {category.Name}");
            }            
            */
        }

        private void ControlledApplication_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            Document document = e.Document;
            string documentId = DocumentServices.GetRevitDocumentGuid(document);
            
            // Cleaning up the Revit Document and AllRegexRules Caches
            if (RevitDocumentCache.ContainsKey(documentId)) { RevitDocumentCache.Remove(documentId); }
            if (AllRegexRules.ContainsKey(documentId)) { AllRegexRules.Remove(documentId); }
        }
        public Result OnShutdown(UIControlledApplication uiControlledApp) { return Result.Succeeded; }
        public class RuleUpdater : IUpdater
        {
            public static bool modelUpdateActive = false;
            AddInId addinID = null;
            UpdaterId updaterID = null;

            public RuleUpdater(AddInId id)
            {
                addinID = id;
                updaterID = new UpdaterId(id, Guid.NewGuid());
            }

            public void Execute(UpdaterData data)
            {
                if (modelUpdateActive == false) { return; }
                Document _document = data.GetDocument();
            }

            public string GetAdditionalInformation() { return "Regular: Reads any Regex Rules in the open document"; }
            public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
            public UpdaterId GetUpdaterId() { return updaterID; }
            public string GetUpdaterName() { return "Regular"; }
        }
    }
}