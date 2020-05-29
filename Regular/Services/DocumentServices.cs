using Autodesk.Revit.DB;

namespace Regular.Services
{
    public static class DocumentServices
    {

        public static string GetRevitDocumentGuid(Document document)
        {
            return ExtensibleStorageServices.GetDocumentGuidFromExtensibleStorage(document);
        }
        public static Document GetRevitDocumentByGuid(string documentGuid)
        {
            if(RegularApp.RevitDocumentCache.ContainsKey(documentGuid)) return RegularApp.RevitDocumentCache[documentGuid];
            return null;
        }
    }
}
