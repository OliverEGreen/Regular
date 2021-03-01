using System;
using System.Collections.Generic;
using System.Linq;
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

        public void AddAndRegisterUpdater(string documentGuid, RegexRule regexRule)
        {
            void RegisterUpdater()
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

            // Adds a RegularUpdater object to the cache
            if (DmUpdaters.ContainsKey(documentGuid))
            {
                if(DmUpdaters[documentGuid].Contains(regexRule.RegularUpdater)) return;
                DmUpdaters[documentGuid].Add(regexRule.RegularUpdater);
            }
            else 
            {
                DmUpdaters.Add(documentGuid, new List<RegularUpdater> { regexRule.RegularUpdater });
            }

            // Registers with the document's updater registry
            RegisterUpdater();
        }
        
        public void RemoveAndDeRegisterUpdater(string documentGuid, RegexRule regexRule)
        {
            void UnregisterUpdater()
            {
                try
                {
                    UpdaterRegistry.UnregisterUpdater(regexRule.RegularUpdater.GetUpdaterId(),
                        RegularApp.DocumentCacheService.GetDocument(documentGuid));
                }
                catch (Exception ex) { TaskDialog.Show("Regular", ex.Message); }

                DmTriggerUtils.DeleteTrigger(documentGuid, regexRule);
            }

            // Removes a RegularUpdater object from the cache
            if (!DmUpdaters.ContainsKey(documentGuid)) return;
            DmUpdaters[documentGuid].Remove(regexRule.RegularUpdater);
            
            // Unregisters from the document's updater registry
            UnregisterUpdater();
        }

        public void AddDocumentUpdaters(string documentGuid)
        {
            // Retrieving the opening document's rules to add triggers to cache and UpdaterRegistry
            List<RegexRule> documentRegexRules = RegularApp.RegexRuleCacheService.GetDocumentRules(documentGuid).ToList();
            documentRegexRules.ForEach(x => AddAndRegisterUpdater(documentGuid, x));
        }

        public void RemoveDocumentUpdaters(string documentGuid)
        {
            // Retrieving the closing document's rules to remove all triggers from cache and UpdaterRegistry
            List<RegexRule> documentRegexRules = RegularApp.RegexRuleCacheService.GetDocumentRules(documentGuid).ToList();
            documentRegexRules.ForEach(x => RemoveAndDeRegisterUpdater(documentGuid, x));
        }

        public RegularUpdater GetUpdater(string documentGuid, UpdaterId updaterId)
        {
            return !DmUpdaters.ContainsKey(documentGuid) ?
                null :
                DmUpdaters[documentGuid].FirstOrDefault(x => x.GetUpdaterId() == updaterId);
        }
    }
}