using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Regular.Services
{
    public static class ExtensibleStorageServices
    {
        public static Schema GetRegularSchema(Document document)
        {
            // A method that handles all the faff of constructing the regularSchema
            Schema ConstructRegularSchema(Document doc)
            {
                // The schema doesn't exist; we need to define the schema for the first time
                SchemaBuilder schemaBuilder = new SchemaBuilder(Guid.NewGuid());
                schemaBuilder.SetSchemaName("RegularSchema");
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);

                // Constructing the scheme for regexRules stored in ExtensibleStorage
                schemaBuilder.AddSimpleField("GUID", typeof(Guid));
                schemaBuilder.AddSimpleField("RuleName", typeof(string));
                schemaBuilder.AddSimpleField("CategoryName", typeof(string));
                schemaBuilder.AddSimpleField("TrackingParameterName", typeof(string));
                schemaBuilder.AddSimpleField("OutputParameterName", typeof(string));
                schemaBuilder.AddSimpleField("RegexString", typeof(string));
                schemaBuilder.AddArrayField("RegexRuleParts", typeof(string));
                return schemaBuilder.Finish();
            }

            IList<Schema> allSchemas = Schema.ListSchemas();
            Schema regularSchema = allSchemas.Where(x => x.SchemaName == "RegularSchema").FirstOrDefault();

            // If it already exists, we return it. If not, we make a new one from scratch
            if (regularSchema != null) return regularSchema;
            return ConstructRegularSchema(document);
        }

        public static void SaveRegexRuleToExtensibleStorage(string documentGuid, RegexRule regexRule)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            //This needs to be turned into a method taking a RegexRule and saving to ExtensibleStorage
            Entity entity = new Entity(GetRegularSchema(document));
            entity.Set("GUID", Guid.NewGuid());
            entity.Set("RuleName", regexRule.RuleName);
            entity.Set("CategoryName", regexRule.TargetCategoryName);
            entity.Set("TrackingParameterName", regexRule.TrackingParameterName);
            entity.Set("OutputParameterName", regexRule.OutputParameterName);
            entity.Set("RegexString", regexRule.RegexString);
            entity.Set<IList<string>>("RegexRuleParts", SerializationServices.SerializeRegexRuleParts(regexRule.RegexRuleParts));
            using (Transaction transaction = new Transaction(document, $"Saving RegexRule {regexRule.RuleName}"))
            {
                transaction.Start();
                DataStorage dataStorage = DataStorage.Create(document);
                dataStorage.SetEntity(entity);
                transaction.Commit();
            }
        }

        public static ObservableCollection<RegexRule> LoadRegexRulesFromExtensibleStorage(Document document)
        {
            // Helper method to take Entities returned from Storage and convert them to RegexRules (including their RegexRuleParts)
            RegexRule ConvertEntityToRegexRule(Document doc, Entity entity)
            {
                string guid = entity.Get<Guid>("GUID").ToString();
                string name = entity.Get<string>("RuleName");
                string categoryName = entity.Get<string>("CategoryName");
                string trackingParameterName = entity.Get<string>("TrackingParameterName");
                string outputParameterName = entity.Get<string>("OutputParameterName");
                string regexString = entity.Get<string>("RegexString");
                List<string> regexRulePartsString = entity.Get<IList<string>>("RegexRuleParts").ToList<string>();
                ObservableCollection<RegexRulePart> regexRuleParts = DeserializationServices.DeserializeRegexRuleParts(regexRulePartsString);

                return new RegexRule(guid, name, categoryName, trackingParameterName, outputParameterName, regexString, regexRuleParts);
            }
            Schema regularSchema = GetRegularSchema(document);

            // Retrieving and testing all DataStorage objects in the document against our Regular schema.
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage == null || allDataStorage.Count < 1) { return null; }

            // Returning any Entities which employ the RegularSchema 
            List<Entity> regexRuleEntities = allDataStorage.Where(x => x.GetEntity(regularSchema) != null).Select(x => x.GetEntity(regularSchema)).ToList();

            ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();
            foreach (Entity entity in regexRuleEntities) { regexRules.Add(ConvertEntityToRegexRule(document, entity)); }
            return regexRules;
        }
        
        public static void DeleteRegexRuleFromExtensibleStorage(string documentGuid, string regexRuleGuid)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            Schema regularSchema = GetRegularSchema(document);

            // Retrieving and testing all DataStorage objects in the document against our Regular schema.
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage == null || allDataStorage.Count < 1) { return; }

            // Returning any Entities which employ the RegularSchema 
            List<DataStorage> regexRuleDataStorage = allDataStorage.Where(x => x.GetEntity(regularSchema) != null).ToList();
            foreach(DataStorage dataStorage in regexRuleDataStorage)
            {
                Entity regexRuleEntity = dataStorage.GetEntity(regularSchema);
                if(regexRuleEntity.Get<Guid>("GUID").ToString() == regexRuleGuid)
                {
                    string ruleName = regexRuleEntity.Get<string>("RuleName");
                    using(Transaction transaction = new Transaction(document, $"Regular - Deleting Rule {ruleName}"))
                    {
                        transaction.Start();
                        List<ElementId> dataStorageToDelete = new List<ElementId>() { dataStorage.Id };
                        document.Delete(dataStorageToDelete);
                        transaction.Commit();
                    }
                    break;
                }
            }
        }
    }
}
