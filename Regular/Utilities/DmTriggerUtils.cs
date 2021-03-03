using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Regular.Models;
using Regular.Services;

namespace Regular.Utilities
{
    public static class DmTriggerUtils
    {
        public static void AddTrigger(string documentGuid, RegexRule regexRule)
        {
            if (regexRule == null) return;

            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            List<BuiltInCategory> targetBuiltInCategories = regexRule.TargetCategoryObjects
                .Where(x => x.IsChecked)
                .Select(x => Category.GetCategory(document, new ElementId(x.CategoryObjectId)))
                .Select(CategoryUtils.GetBuiltInCategoryFromCategory)
                .ToList();
            
            ElementId trackingParameterId = new ElementId(regexRule.TrackingParameterObject.ParameterObjectId);

            RegularApp.DmUpdaterCacheService.AddAndRegisterUpdater(documentGuid, regexRule);

            UpdaterRegistry.AddTrigger(
                regexRule.RegularUpdater.GetUpdaterId(),
                document,
                new ElementMulticategoryFilter(targetBuiltInCategories),
                Element.GetChangeTypeParameter(trackingParameterId));
        }

        public static void DeleteTrigger(string documentGuid, RegexRule regexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            UpdaterRegistry.RemoveDocumentTriggers(regexRule.RegularUpdater.GetUpdaterId(), document);
        }

        public static void UpdateTrigger(string documentGuid, RegexRule regexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            UpdaterId updaterId = regexRule.RegularUpdater.GetUpdaterId();
            string updaterIdString = updaterId.GetGUID().ToString();
            UpdaterRegistry.RemoveDocumentTriggers(updaterId, document);
            //RegularApp.DmUpdaterCacheService.RemoveAndDeRegisterUpdater(documentGuid, regexRule);
            AddTrigger(documentGuid, regexRule);
        }
    }
}
