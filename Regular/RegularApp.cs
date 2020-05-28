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
using Autodesk.Revit.DB.Structure;
using System.Collections.ObjectModel;
using Regular;
using Regular.Models;
using Regular.Services;
using System.Windows;

namespace Regular
{
    public class RegularApp : IExternalApplication
    {
        public static Dictionary<string, ObservableCollection<RegexRule>> AllRegexRules { get; set; }
        public static Application RevitApplication { get; set; }
        public Result OnStartup(UIControlledApplication uiControlledApp)
        {
            RegularRibbon.BuildRegularRibbon(uiControlledApp);
            
            // This keeps track of sets of rules per document, so is initialized as soon as possible
            AllRegexRules = new Dictionary<string, ObservableCollection<RegexRule>>();
            
            // Events which will add to or clean up the AllRegexRules cache
            uiControlledApp.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;
            uiControlledApp.ControlledApplication.DocumentClosing += ControlledApplication_DocumentClosing;
            return Result.Succeeded;
        }
        private void ControlledApplication_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            Document document = e.Document;
            RevitApplication = document.Application;
            string documentId = DocumentServices.GetRevitDocumentGuid(document);
            // Creates an accessible, stable reference to the Revit document
            DocumentServices.RevitDocumentCache[documentId] = document;

            // We retrieve any existing rules from ExtensibleStorage and save them to a static cache.
            if (AllRegexRules.ContainsKey(documentId)) { Utilities.LoadRegexRulesFromExtensibleStorage(document, document.Application); }
            // If none are found, our static cache is an empty ObservableCollection
            else { AllRegexRules[documentId] = AllRegexRules[documentId] = new ObservableCollection<RegexRule>(); }

            ObservableCollection<RegexRule> documentRegexRules = AllRegexRules[documentId];
            // If there are no saved rules we return
            if (documentRegexRules.Count < 1) { return; }
            
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
            if (DocumentServices.RevitDocumentCache.ContainsKey(documentId)) { DocumentServices.RevitDocumentCache.Remove(documentId); }
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