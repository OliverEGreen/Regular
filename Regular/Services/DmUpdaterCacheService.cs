using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Regular.Models;
using Regular.Utilities;

namespace Regular.Services
{
    public class DmUpdaterCacheService
    {
        // Singleton Service class handling all Dynamic Model Updaters for the application instance
        private static DmUpdaterCacheService DmUpdaterCacheServiceInstance { get; set; }
        private static Dictionary<string, List<RegularUpdater>> DmUpdaters { get; set; }

        private DmUpdaterCacheService() { }
        public static DmUpdaterCacheService Instance()
        {
            if (DmUpdaterCacheServiceInstance != null) return DmUpdaterCacheServiceInstance;
            DmUpdaterCacheServiceInstance = new DmUpdaterCacheService();
            DmUpdaters = new Dictionary<string, List<RegularUpdater>>();
            return DmUpdaterCacheServiceInstance;
        }

        public void AddUpdater(string documentGuid, RegexRule regexRule)
        {
            // Adds a RegularUpdater object to the cache
            if (DmUpdaters.ContainsKey(documentGuid) && !DmUpdaters[documentGuid].Contains(regexRule.RegularUpdater))
            {
                DmUpdaters[documentGuid].Add(regexRule.RegularUpdater);
            }
            else 
            {
                DmUpdaters.Add(documentGuid, new List<RegularUpdater> { regexRule.RegularUpdater });
            }
            RegisterUpdaterWithDmu(documentGuid, regexRule);
        }

        public void RegisterUpdaterWithDmu(string documentGuid, RegexRule regexRule)
        {
            // Using the optional boolean flag so the updater doesn't pop up with a massive scary message on loading
            try
            {
                UpdaterRegistry.RegisterUpdater(
                    GetUpdater(documentGuid, regexRule.RegularUpdater.GetUpdaterId()),
                    RegularApp.DocumentCacheService.GetDocument(documentGuid),
                    true);
            }
            catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }

            DmTriggerUtils.AddTrigger(documentGuid, regexRule);
        }

        public void RemoveUpdater(string documentGuid, RegexRule regexRule)
        {
            if (!DmUpdaters.ContainsKey(documentGuid)) return;
            
            try
            {
                UpdaterRegistry.UnregisterUpdater(
                    regexRule.RegularUpdater.GetUpdaterId(),
                    RegularApp.DocumentCacheService.GetDocument(documentGuid));

            }
            catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }

            DmUpdaters[documentGuid].Remove(regexRule.RegularUpdater);
            DmTriggerUtils.DeleteTrigger(documentGuid, regexRule);
        }

        public void AddDocumentUpdaters(string documentGuid)
        {
            // Retrieving the opening document's rules to add triggers to cache and UpdaterRegistry
            List<RegexRule> documentRegexRules = RegularApp.RegexRuleCacheService.GetDocumentRules(documentGuid).ToList();
            documentRegexRules.ForEach(x => AddUpdater(documentGuid, x));
        }

        public void RemoveDocumentUpdaters(string documentGuid)
        {
            // Retrieving the closing document's rules to remove all triggers from cache and UpdaterRegistry
            List<RegexRule> documentRegexRules = RegularApp.RegexRuleCacheService.GetDocumentRules(documentGuid).ToList();
            documentRegexRules.ForEach(x => RemoveUpdater(documentGuid, x));
        }

        public RegularUpdater GetUpdater(string documentGuid, UpdaterId updaterId)
        {
            return !DmUpdaters.ContainsKey(documentGuid) ?
                null :
                DmUpdaters[documentGuid].FirstOrDefault(x => x.GetUpdaterId() == updaterId);
        }
    }
}