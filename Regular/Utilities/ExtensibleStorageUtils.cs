using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Regular.Models;

namespace Regular.Utilities
{
    public static class ExtensibleStorageUtils
    {
        // CRUD Services for managing Regular rule data stored using Revit's ExtensibleStorage API
        
        private static Schema GetRegularSchema()
        {
            // A method that handles constructing the regularSchema
            Schema ConstructRegularSchema()
            {
                // The schema doesn't exist; we need to define the schema for the first time
                SchemaBuilder schemaBuilder = new SchemaBuilder(Guid.NewGuid());
                schemaBuilder.SetSchemaName("RegularSchema");
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
                // TODO: Sort out these permissions
                //schemaBuilder.SetReadAccessLevel(AccessLevel.Application);
                //schemaBuilder.SetWriteAccessLevel(AccessLevel.Application);
                //schemaBuilder.SetVendorId("OGRN");
                // Constructing the scheme for regexRules stored in ExtensibleStorage
                schemaBuilder.AddSimpleField("SerializedRegexRule", typeof(string));
                return schemaBuilder.Finish();
            }

            IList<Schema> allSchemas = Schema.ListSchemas();
            Schema regularSchema = allSchemas.FirstOrDefault(x => x.SchemaName == "RegularSchema");

            // If it already exists, we return it. If not, we make a new one from scratch
            return regularSchema ?? ConstructRegularSchema();
        }
        public static void SaveRegexRuleToExtensibleStorage(string documentGuid, RegexRule regexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            //This needs to be turned into a method taking a RegexRule and saving to ExtensibleStorage
            Entity entity = new Entity(GetRegularSchema());
            string serializedRegexRule = SerializationUtils.SerializeRegexRule(regexRule);
            entity.Set("SerializedRegexRule", serializedRegexRule);
            using (Transaction transaction = new Transaction(document, $"Saving RegexRule {regexRule.RuleName}"))
            {
                transaction.Start();
                DataStorage dataStorage = DataStorage.Create(document);
                dataStorage.SetEntity(entity);
                transaction.Commit();
            }
        }
        public static ObservableCollection<RegexRule> GetAllRegexRulesInExtensibleStorage(string documentGuid)
        {
            // Helper method to take Entities returned from Storage and convert them to RegexRules (including their RegexRuleParts)
            RegexRule ConvertEntityToRegexRule(Entity entity)
            {
                string serializedRegexRule = entity.Get<string>("SerializedRegexRule");
                RegexRule deserializedRegexRule = SerializationUtils.DeserializeRegexRule(serializedRegexRule);
                return deserializedRegexRule;
            }
            Schema regularSchema = GetRegularSchema();

            // Retrieving and testing all DataStorage objects in the document against our Regular schema.
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage.Count < 1) { return null; }

            // Returning any Entities which employ the RegularSchema 
            List<Entity> regexRuleEntities = allDataStorage.Where(x => x.GetEntity(regularSchema).IsValid()).Select(x => x.GetEntity(regularSchema)).ToList();

            ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();
            try
            {
                foreach (Entity entity in regexRuleEntities) { regexRules.Add(ConvertEntityToRegexRule(entity)); }
            }
            catch { }

            return regexRules;
        }
        public static void UpdateRegexRuleInExtensibleStorage(string documentGuid, string regexRuleGuid, RegexRule newRegexRule)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            KeyValuePair<DataStorage, Entity> ruleInExtensibleStorage = GetRegexRuleInExtensibleStorage(documentGuid, regexRuleGuid);
            Entity regexRuleEntity = ruleInExtensibleStorage.Value;
            if (regexRuleEntity == null) return;
            DataStorage dataStorage = ruleInExtensibleStorage.Key;

            string serializedRegexRule = regexRuleEntity.Get<string>("SerializedRegexRule");
            RegexRule regexRule = SerializationUtils.DeserializeRegexRule(serializedRegexRule);
            string previousName = regexRule.RuleName;

            using (Transaction transaction = new Transaction(document, $"Regular - Modifying Rule {previousName}"))
            {
                transaction.Start();
                regexRuleEntity.Set("SerializedRegexRule", SerializationUtils.SerializeRegexRule(newRegexRule));
                dataStorage.SetEntity(regexRuleEntity);
                transaction.Commit();
            }
        }
        public static void DeleteRegexRuleFromExtensibleStorage(string documentGuid, string regexRuleGuid)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            KeyValuePair<DataStorage, Entity> ruleInExtensibleStorage = GetRegexRuleInExtensibleStorage(documentGuid, regexRuleGuid);
            Entity regexRuleEntity = ruleInExtensibleStorage.Value;
            if (regexRuleEntity == null) return;
            DataStorage dataStorage = ruleInExtensibleStorage.Key;

            string serializedRegexRule = regexRuleEntity.Get<string>("SerializedRegexRule");
            RegexRule regexRule = SerializationUtils.DeserializeRegexRule(serializedRegexRule);
            string ruleName = regexRule.RuleName;

            using (Transaction transaction = new Transaction(document, $"Regular - Deleting Rule {ruleName}"))
            {
                transaction.Start();
                List<ElementId> dataStorageToDelete = new List<ElementId> { dataStorage.Id };
                document.Delete(dataStorageToDelete);
                transaction.Commit();
            }
        }
        private static KeyValuePair<DataStorage, Entity> GetRegexRuleInExtensibleStorage(string documentGuid, string regexRuleGuid)
        {
            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);
            Schema regularSchema = GetRegularSchema();

            // Retrieving and testing all DataStorage objects in the document against our Regular schema.
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage.Count < 1) { return new KeyValuePair<DataStorage, Entity>(null, null); }

            // Returning any Entities which employ the RegularSchema 
            List<DataStorage> regexRuleDataStorage = allDataStorage.Where(x => x.GetEntity(regularSchema).IsValid()).ToList();
            foreach (DataStorage dataStorage in regexRuleDataStorage)
            {
                Entity regexRuleEntity = dataStorage.GetEntity(regularSchema);
                string serializedRegexRule = regexRuleEntity.Get<string>("SerializedRegexRule");
                RegexRule regexRule = SerializationUtils.DeserializeRegexRule(serializedRegexRule);
                // If the rule has the right GUID, we return the DataStorage and Entity objects to be worked with
                if(regexRule.RuleGuid == regexRuleGuid) return new KeyValuePair<DataStorage, Entity>(dataStorage, regexRuleEntity);
            }
            // If the rule wasn't found, we return nothing
            return new KeyValuePair<DataStorage, Entity>(null, null);
        }
    }
}
