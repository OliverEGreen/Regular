using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Regular.Utilities
{
    public static class DocumentGuidUtils
    {
        // Each Revit document needs a unique ID as a reference, enabling us to handle multiple Revit documents within the same session
        // Since the Revit API doesn't provide such a unique ID, we generate a new GUID and save it to each doucment's ExtensibleStorage
        // Static methods in this class are a centralised way of referring to and retrieving documents by their GUID.
        private static Schema GetRegularDocumentGUIDSchema()
        {
            // Returns a schema for a document GUID. If none is found, it creates one and then returns it
            Schema ConstructGuidSchema()
            {
                SchemaBuilder schemaBuilder = new SchemaBuilder(Guid.NewGuid());
                schemaBuilder.SetSchemaName("RegularDocumentGUIDSchema");
                schemaBuilder.SetApplicationGUID(RegularApp.RegularApplicationGUID);
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
                schemaBuilder.SetVendorId("OGRN");

                // Constructing the scheme for regexRules stored in ExtensibleStorage
                schemaBuilder.AddSimpleField("RegularDocumentGUID", typeof(string));
                return schemaBuilder.Finish();
            }
            IList<Schema> allSchemas = Schema.ListSchemas();
            Schema regularDocumentGUIDSchema = allSchemas.FirstOrDefault(x => x.SchemaName == "RegularDocumentGUIDSchema");

            // If it already exists, we return it. If not, we make a new one from scratch
            return regularDocumentGUIDSchema ?? ConstructGuidSchema();
        }
        
        public static string GetDocumentGuidFromExtensibleStorage(Document document)
        {
            Schema guidSchema = GetRegularDocumentGUIDSchema();

            // Retrieving and testing all DataStorage objects in the document against our DocumentGuid schema.
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage.Count < 1) return null;

            // Returning the document RuleGuid if the schema is employed, otherwise null
            DataStorage documentGuidDataStorage = allDataStorage.FirstOrDefault(x => x.GetEntity(guidSchema).IsValid());
            Entity documentGuidEntity = documentGuidDataStorage?.GetEntity(guidSchema);
            string documentGuid = documentGuidEntity?.Get<string>("RegularDocumentGUID");
            return documentGuid;
        }
        
        public static string RegisterDocumentGuidToExtensibleStorage(Document document)
        {
            Entity entity = new Entity(GetRegularDocumentGUIDSchema());
            string documentGuid = Guid.NewGuid().ToString();
            entity.Set("RegularDocumentGUID", documentGuid);
            using (Transaction transaction = new Transaction(document, "Saving Document Reference GUID"))
            {
                transaction.Start();
                DataStorage dataStorage = DataStorage.Create(document);
                dataStorage.SetEntity(entity);
                transaction.Commit();
            }
            return documentGuid;
        }
    }
}
