using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Regular.Models;

namespace Regular.Utilities
{
    public static class DmTriggerUtils
    {
        internal static void AddAllTriggers(string documentGuid, ObservableCollection<RegexRule> regexRules)
        {
            foreach (RegexRule regexRule in regexRules) AddTrigger(documentGuid, regexRule);
        }
        
        public static void AddTrigger(string documentGuid, RegexRule regexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            List<ElementId> targetCategoryIds = regexRule.TargetCategoryObjects.Select(x => new ElementId(Convert.ToInt32((int) x.CategoryObjectId))).ToList();
            List<Category> targetCategories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).ToList();
            List<BuiltInCategory> targetBuiltInCategories = targetCategories.Select(CategoryUtils.GetBuiltInCategoryFromCategory).ToList();
            ElementId trackingParameterId = new ElementId(regexRule.TrackingParameterObject.ParameterObjectId);

            UpdaterRegistry.AddTrigger(
                RegularApp.DmUpdaterCacheService.GetUpdater(documentGuid).GetUpdaterId(),
                document,
                new ElementMulticategoryFilter(targetBuiltInCategories),
                Element.GetChangeTypeParameter(trackingParameterId));
        }

        public static void DeleteTrigger(string documentGuid, RegexRule ruleToRemoveTriggerFrom)
        {
            // There is no way to delete a specific trigger so we must remove all document-based triggers
            // and recreate them minus the one trigger we're removing.
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            UpdaterId updaterId = RegularApp.DmUpdaterCacheService.GetUpdater(documentGuid).GetUpdaterId();
            UpdaterRegistry.RemoveDocumentTriggers(updaterId, document);
            foreach (RegexRule regexRule in RegularApp.RegexRuleCacheService.GetDocumentRules(documentGuid))
            {
                // We don't add back the trigger for the rule we're removing
                if (regexRule.RuleGuid == ruleToRemoveTriggerFrom.RuleGuid) continue;
                AddTrigger(documentGuid, regexRule);
            }
        }

        public static void UpdateAllTriggers(string documentGuid)
        {
            // There is no way to delete a specific trigger so we must remove all document-based triggers
            // and recreate all of them one by one, but with the new RegexRuleInfo
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            UpdaterId updaterId = RegularApp.DmUpdaterCacheService.GetUpdater(documentGuid).GetUpdaterId();
            UpdaterRegistry.RemoveDocumentTriggers(updaterId, document);
            foreach (RegexRule regexRule in RegularApp.RegexRuleCacheService.GetDocumentRules(documentGuid))
            {
                // We recreate all of the triggers
                AddTrigger(documentGuid, regexRule);
            }
        }
    }
}
