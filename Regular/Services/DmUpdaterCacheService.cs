using System.Collections.Generic;

namespace Regular.Services
{
    public class DmUpdaterCacheService
    {
        // Singleton Service class handling all Dynamic Model Updaters for the application instance
        private static DmUpdaterCacheService DmUpdaterCacheServiceInstance { get; set; }
        private static Dictionary<string, RegularUpdater> DmUpdaters { get; set; }

        private DmUpdaterCacheService() { }
        public static DmUpdaterCacheService Instance()
        {
            if (DmUpdaterCacheServiceInstance != null) return DmUpdaterCacheServiceInstance;
            DmUpdaterCacheServiceInstance = new DmUpdaterCacheService();
            DmUpdaters = new Dictionary<string, RegularUpdater>();
            return DmUpdaterCacheServiceInstance;
        }

        public void AddUpdater(string documentGuid, RegularUpdater updater)
        {
            // Any previous updaters for the document are removed (shouldn't happen in theory)
            if (DmUpdaters.ContainsKey(documentGuid)) DmUpdaters.Remove(documentGuid);
            DmUpdaters[documentGuid] = updater;
        }
        public void RemoveUpdater(string documentGuid)
        {
            if (!DmUpdaters.ContainsKey(documentGuid)) return;
            DmUpdaters.Remove(documentGuid);
        }

        public RegularUpdater GetUpdater(string documentGuid)
        {
            return !DmUpdaters.ContainsKey(documentGuid) ? null : DmUpdaters[documentGuid];
        }
    }
}