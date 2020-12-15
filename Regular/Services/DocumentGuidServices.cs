using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Regular.Services
{
    public static class DocumentGuidServices
    {
        // Each Revit document needs a unique ID as a reference, enabling us to handle multiple Revit documents within the same session
        // Since the Revit API doesn't provide such a unique ID, we generate a new GUID and save it to each doucment's ExtensibleStorage
        // Static methods in this class are a centralised way of referring to and retrieving documents by their GUID.
        
        public static string GetDocumentGuidFromExtensibleStorage(Document document)
        {
            Schema guidSchema = GetDocumentGuidSchema();

            // Retrieving and testing all DataStorage objects in the document against our DocumentGuid schema.
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage.Count < 1) return null;

            // Returning the document RuleGuid if the schema is employed, otherwise null
            DataStorage documentGuidDataStorage = allDataStorage.FirstOrDefault(x => x.GetEntity(guidSchema).IsValid());
            Entity documentGuidEntity = documentGuidDataStorage?.GetEntity(guidSchema);
            return documentGuidEntity?.Get<string>("DocumentGUID");
        }
        
        public static Document GetRevitDocumentByGuid(string documentGuid)
        {
            // Takes a GUID, returns a document from the cache if registered
            return RegularApp.RevitDocumentCache.ContainsKey(documentGuid) ? RegularApp.RevitDocumentCache[documentGuid] : null;
        }

        private static Schema GetDocumentGuidSchema()
        {
            // Returns a schema for a document GUID. If none is found, it creates one and then returns it
            Schema ConstructGuidSchema()
            {
                SchemaBuilder schemaBuilder = new SchemaBuilder(Guid.NewGuid());
                schemaBuilder.SetSchemaName("DocumentGUIDSchema");
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);

                // Constructing the scheme for regexRules stored in ExtensibleStorage
                schemaBuilder.AddSimpleField("DocumentGUID", typeof(string));
                return schemaBuilder.Finish();
            }
            IList<Schema> allSchemas = Schema.ListSchemas();
            Schema documentGuidSchema = allSchemas.FirstOrDefault(x => x.SchemaName == "DocumentGUIDSchema");

            // If it already exists, we return it. If not, we make a new one from scratch
            return documentGuidSchema ?? ConstructGuidSchema();
        }

        public static string RegisterDocumentGuidToExtensibleStorage(Document document)
        {
            Entity entity = new Entity(GetDocumentGuidSchema());
            string newGuidString = Guid.NewGuid().ToString();
            entity.Set("DocumentGUID", newGuidString);
            using (Transaction transaction = new Transaction(document, "Saving Document Reference GUID"))
            {
                transaction.Start();
                DataStorage dataStorage = DataStorage.Create(document);
                dataStorage.SetEntity(entity);
                transaction.Commit();
            }
            return newGuidString;
        }
    }
}
