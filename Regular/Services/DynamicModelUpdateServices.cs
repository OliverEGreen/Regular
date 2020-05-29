using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using static Regular.RegularApp;

namespace Regular.Services
{
    public static class DynamicModelUpdateServices
    {
        private class RuleUpdater : IUpdater
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
                Document document = data.GetDocument();
                string documentGuid = DocumentServices.GetRevitDocumentGuid(document);
                List<ElementId> modifiedElementIds = data.GetModifiedElementIds().ToList();
                RunAllRegexRules(documentGuid, modifiedElementIds);                
            }

            public string GetAdditionalInformation() { return "Regular: Reads any Regex Rules in the open document"; }
            public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
            public UpdaterId GetUpdaterId() { return updaterID; }
            public string GetUpdaterName() { return "Regular Updater"; }
        }
        public static void RunAllRegexRules(string documentGuid, List<ElementId> modifiedElementIds)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            List<RegexRule> regexRules = RegexRuleManager.GetDocumentRegexRules(documentGuid).ToList();

            // Using a dictionary to group modified Elements together by their Category
            Dictionary<Category, List<Element>> modifiedElementsByCategory = new Dictionary<Category, List<Element>>();

            foreach (ElementId elementId in modifiedElementIds)
            {
                Element element = document.GetElement(elementId);
                if (element == null) continue;
                // If we can retrieve the element, we'll add it to the dictionary
                if (modifiedElementsByCategory.ContainsKey(element.Category)) { modifiedElementsByCategory[element.Category].Add(element); }
                else { modifiedElementsByCategory[element.Category] = new List<Element> { element }; }
            }

            // All of the category names which were affected
            List<string> modifiedCategoryNames = modifiedElementsByCategory.Keys.Select(x => x.Name).Distinct().ToList();

            foreach (RegexRule regexRule in regexRules)
            {
                // If the regexRule applies to an irrelevant Category, we pass over it
                if (!modifiedCategoryNames.Contains(regexRule.TargetCategoryName)) continue;

                ElementCategoryFilter elementCategoryFilter = new ElementCategoryFilter(CategoryServices.GetBuiltInCategoryFromCategory(CategoryServices.GetCategoryByName(document, regexRule.TargetCategoryName)));
                List<Element> elementsOfTargetCategory = new FilteredElementCollector(document).WherePasses(elementCategoryFilter).WhereElementIsNotElementType().ToList();
                
                if (elementsOfTargetCategory == null || elementsOfTargetCategory.Count < 1) { continue; }
                foreach (Element element in elementsOfTargetCategory)
                {
                    Parameter trackingParameter = element.LookupParameter(regexRule.TrackingParameterName);
                    Parameter outputParameter = element.LookupParameter(regexRule.OutputParameterName);
                    if (trackingParameter == null || outputParameter == null) { continue; }
                    Regex regex = new Regex(regexRule.RegexString);
                    if (regex.IsMatch(trackingParameter.ToString())) { outputParameter.Set("Valid"); }
                    else { outputParameter.Set("Invalid"); }
                }
            }
        }

        public static void RegisterRegexRule(string documentGuid, string regexRuleGuid)
        {
            RuleUpdater ruleUpdater = new RuleUpdater(RevitApplication.ActiveAddInId);

            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            try { UpdaterRegistry.RegisterUpdater(ruleUpdater, document); }
            catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }

            RegexRule regexRule = RegexRuleManager.GetRegexRule(documentGuid, regexRuleGuid);
            BuiltInCategory builtInCategory = CategoryServices.GetBuiltInCategoryFromCategory(CategoryServices.GetCategoryByName(document, regexRule.TargetCategoryName));
            ElementCategoryFilter elementCategoryFilter = new ElementCategoryFilter(builtInCategory);
            // A bit of a fudge - can't figure out how to get the Mark parameter without an element yet...
            Element element = new FilteredElementCollector(document).WherePasses(elementCategoryFilter).WhereElementIsNotElementType().ToElements().ToList().FirstOrDefault();
            if (element == null) return;
            Parameter targetParameter = element.LookupParameter(regexRule.TrackingParameterName);
            if (targetParameter == null) return;
            UpdaterRegistry.AddTrigger(ruleUpdater.GetUpdaterId(), elementCategoryFilter, Element.GetChangeTypeParameter(targetParameter.Id));
            List<Element> elements = new FilteredElementCollector(document).OfCategory(builtInCategory).ToElements().ToList();
            TaskDialog.Show("Test", $"Found {elements.Count} elements of category {regexRule.TargetCategoryName}");
        }
    }
}
