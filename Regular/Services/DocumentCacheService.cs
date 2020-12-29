using System.Collections.Generic;
using Document = Autodesk.Revit.DB.Document;

namespace Regular.Services
{
    public class DocumentCacheService
    {
        // Singleton Service class handling all Revit Documents open in the application instance
        private static DocumentCacheService DocumentCacheServiceInstance { get; set; }
        private static Dictionary<string, Document> Documents { get; set; }

        private DocumentCacheService() { }
        public static DocumentCacheService Instance()
        {
            if (DocumentCacheServiceInstance != null) return DocumentCacheServiceInstance;
            DocumentCacheServiceInstance = new DocumentCacheService();
            Documents = new Dictionary<string, Document>();
            return DocumentCacheServiceInstance;
        }

        public void AddDocument(string documentGuid, Document document)
        {
            if (Documents.ContainsKey(documentGuid)) return;
            {
                Documents[documentGuid] = document;
            }
        }
        public void RemoveDocument(string documentGuid)
        {
            if (!Documents.ContainsKey(documentGuid)) return;
            Documents.Remove(documentGuid);
        }

        public Document GetDocument(string documentGuid)
        {
            return Documents.ContainsKey(documentGuid) ? Documents[documentGuid] : null;
        }
    }
}
