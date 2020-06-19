using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Regular.ViewModel;
using static Regular.RegularApp;

namespace Regular.Services
{
    public static class DynamicModelUpdateServices
    {
        private class RuleUpdater : IUpdater
        {
            private readonly UpdaterId updaterId = null;

            public RuleUpdater(AddInId id)
            {
                updaterId = new UpdaterId(id, Guid.NewGuid());
            }

            public void Execute(UpdaterData data)
            {
                Document document = data.GetDocument();
                TaskDialog.Show("Test", "DMU Executing");
                string documentGuid = DocumentServices.GetRevitDocumentGuid(document);
                List<ElementId> modifiedElementIds = data.GetModifiedElementIds().ToList();
                RunAllRegexRules(documentGuid, modifiedElementIds);                
            }
            public string GetAdditionalInformation() { return "Regular: Reads any Regex Rules in the open document"; }
            public ChangePriority GetChangePriority() { return ChangePriority.Annotations; }
            public UpdaterId GetUpdaterId() { return updaterId; }
            public string GetUpdaterName() { return "Regular Updater"; }
        }
        public static void RunAllRegexRules(string documentGuid, List<ElementId> modifiedElementIds)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            List<RegexRule> regexRules = RegexRule.GetDocumentRegexRules(documentGuid).ToList();

            List<Element> modifiedElements = modifiedElementIds.Select(x => document.GetElement(x)).ToList();
            List<string> modifiedCategoryIds = modifiedElements.Select(x => x.Category.Id.ToString()).Distinct().ToList();

            List<RegexRule> regexRulesToRun = new List<RegexRule>();

            // Trying to figure out which Regex rules to validate against given the modified element Ids
            foreach(RegexRule regexRule in regexRules)
            {
                List<string> targetCategoryIds = regexRule.TargetCategoryIds.Select(x => x.Id).ToList();
                if (targetCategoryIds.Any(x => modifiedCategoryIds.Contains(x))) regexRulesToRun.Add(regexRule);
            }

            foreach (RegexRule regexRule in regexRulesToRun)
            {
                List<ElementId> targetCategoryIds = regexRule.TargetCategoryIds.Select(x => new ElementId(Convert.ToInt32(x))).ToList();
                List<Category> targetCategories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).ToList();
                List<BuiltInCategory> targetBuiltInCategories = targetCategories.Select(CategoryServices.GetBuiltInCategoryFromCategory).ToList();

                // Creating a MultiCategoryFilter to target the updater trigger
                ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(targetBuiltInCategories);
                List<Element> elementsOfTargetCategory = new FilteredElementCollector(document).WherePasses(elementMulticategoryFilter).WhereElementIsNotElementType().ToList();
                
                if (elementsOfTargetCategory.Count < 1) { continue; }
                foreach (Element element in elementsOfTargetCategory)
                {
                    Parameter trackingParameter = element.LookupParameter(regexRule.TrackingParameterName);
                    Parameter outputParameter = element.LookupParameter(regexRule.OutputParameterName);
                    if (trackingParameter == null || outputParameter == null) { continue; }
                    Regex regex = new Regex(regexRule.RegexString);
                    outputParameter.Set(regex.IsMatch(trackingParameter.AsString()) ? "Valid" : "Invalid");
                }
            }
        }

        public static void RegisterRegexRuleUpdater(string documentGuid, string regexRuleGuid)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            RegexRule regexRule = RegexRule.GetRuleById(documentGuid, regexRuleGuid);

            RuleUpdater ruleUpdater = new RuleUpdater(RevitApplication.ActiveAddInId);

            // This is the only place that this gets set to the rule
            regexRule.UpdaterId = ruleUpdater.GetUpdaterId();

            // Using the optional boolean flag so the updater doesn't pop up with a massive scary message on loading
            try { UpdaterRegistry.RegisterUpdater(ruleUpdater, document, true); }
            catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }
            
            // Converting the save string representations of the Target Category ElementIds to ElementIds. Maybe can save these as integers and skip the conversion.
            List<ElementId> targetCategoryIds = regexRule.TargetCategoryIds.Select(x => new ElementId(Convert.ToInt32(x.Id))).ToList();
            List<Category> targetCategories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).ToList();
            List<BuiltInCategory> targetBuiltInCategories = targetCategories.Select(CategoryServices.GetBuiltInCategoryFromCategory).ToList();
            
            // Creating a MultiCategoryFilter to target the updater trigger
            ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(targetBuiltInCategories);
            
            // Adding the trigger to the updater registry
            Parameter trackingParameter = ParameterServices.GetProjectParameterByName(document, regexRule.TrackingParameterName);
            if (trackingParameter == null) return;
            UpdaterRegistry.AddTrigger(ruleUpdater.GetUpdaterId(), elementMulticategoryFilter, Element.GetChangeTypeParameter(trackingParameter));
        }

        public static void DeleteRegexRuleUpdater(string documentGuid, string regexRuleGuid)
        {
            RegexRule regexRule = RegexRule.GetRuleById(documentGuid, regexRuleGuid);
            
            // Double-checking that the updater was ever registered before attempting to remove
            if (regexRule.UpdaterId == null) return;
            UpdaterRegistry.UnregisterUpdater(regexRule.UpdaterId);
        }

        public static void UpdateRegexRuleUpdater(string documentGuid, string regexRuleGuid)
        {
            // We need to check whether this updater already exists.
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            RegexRule regexRule = RegexRule.GetRuleById(documentGuid, regexRuleGuid);

            if (regexRule.UpdaterId == null) return;
            UpdaterRegistry.UnregisterUpdater(regexRule.UpdaterId);
        }
    }
}
