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

            List<BuiltInCategory> targetBuiltInCategories = regexRule.TargetCategoryObjects
                .Where(x => x.IsChecked)
                .Select(x => Category.GetCategory(document, new ElementId(x.CategoryObjectId)))
                .Select(CategoryUtils.GetBuiltInCategoryFromCategory)
                .ToList();
            
            ElementId trackingParameterId = new ElementId(regexRule.TrackingParameterObject.ParameterObjectId);

            UpdaterRegistry.AddTrigger(
                regexRule.UpdaterId,
                document,
                new ElementMulticategoryFilter(targetBuiltInCategories),
                Element.GetChangeTypeParameter(trackingParameterId));
        }

        public static void DeleteTrigger(string documentGuid, RegexRule regexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            UpdaterRegistry.RemoveDocumentTriggers(regexRule.UpdaterId, document);
        }

        public static void UpdateTrigger(string documentGuid, RegexRule regexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            UpdaterId updaterId = regexRule.UpdaterId;
            UpdaterRegistry.RemoveDocumentTriggers(updaterId, document);
            AddTrigger(documentGuid, regexRule);
        }
    }
}
