using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Regular.Services
{
    public static class DocumentServices
    {
        public static Dictionary<string, Document> RevitDocumentCache = new Dictionary<string, Document>();
        public static string GetRevitDocumentGuid(Document document) { return document.ProjectInformation.UniqueId; }
        public static Document GetRevitDocumentByGuid(string documentGuid)
        {
            if(RevitDocumentCache.ContainsKey(documentGuid)) return RevitDocumentCache[documentGuid];
            return null;
        }
    }
}
