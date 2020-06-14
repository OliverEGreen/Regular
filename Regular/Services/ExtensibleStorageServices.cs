using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Regular.Enums;
using Regular.ViewModel;

namespace Regular.Services
{
    public static class ExtensibleStorageServices
    {
        // CRUD Services for managing Regular data stored using Revit's ExtensibleStorage API
        private static Schema GetDocumentGuidSchema(Document document)
        {
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
            Entity entity = new Entity(GetDocumentGuidSchema(document));
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
        public static string GetDocumentGuidFromExtensibleStorage(Document document)
        {
            Schema guidSchema = GetDocumentGuidSchema(document);
            
            // Retrieving and testing all DataStorage objects in the document against our DocumentGuid schema.
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage.Count < 1) return null;

            // Returning the document Guid if the schema is employed, otherwise null
            DataStorage documentGuidDataStorage = allDataStorage.FirstOrDefault(x => x.GetEntity(guidSchema).IsValid());
            Entity documentGuidEntity = documentGuidDataStorage?.GetEntity(guidSchema);
            return documentGuidEntity?.Get<string>("DocumentGUID");
        }
        private static Schema GetRegularSchema()
        {
            // A method that handles all the faff of constructing the regularSchema
            Schema ConstructRegularSchema()
            {
                // The schema doesn't exist; we need to define the schema for the first time
                SchemaBuilder schemaBuilder = new SchemaBuilder(Guid.NewGuid());
                schemaBuilder.SetSchemaName("RegularSchema");
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);

                // Constructing the scheme for regexRules stored in ExtensibleStorage
                schemaBuilder.AddSimpleField("GUID", typeof(Guid));
                schemaBuilder.AddSimpleField("RuleName", typeof(string));
                schemaBuilder.AddArrayField("TargetCategoryIds", typeof(string));
                schemaBuilder.AddSimpleField("TrackingParameterName", typeof(string));
                schemaBuilder.AddSimpleField("OutputParameterName", typeof(string));
                schemaBuilder.AddSimpleField("RegexString", typeof(string));
                schemaBuilder.AddSimpleField("MatchTypes", typeof(string));
                schemaBuilder.AddArrayField("RegexRuleParts", typeof(string));
                return schemaBuilder.Finish();
            }

            IList<Schema> allSchemas = Schema.ListSchemas();
            Schema regularSchema = allSchemas.FirstOrDefault(x => x.SchemaName == "RegularSchema");

            // If it already exists, we return it. If not, we make a new one from scratch
            return regularSchema ?? ConstructRegularSchema();
        }
        public static void SaveRegexRuleToExtensibleStorage(string documentGuid, RegexRule regexRule)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            //This needs to be turned into a method taking a RegexRule and saving to ExtensibleStorage
            Entity entity = new Entity(GetRegularSchema());
            entity.Set("GUID", new Guid(regexRule.Guid));
            entity.Set("RuleName", regexRule.Name);
            entity.Set("TargetCategoryIds", SerializationServices.ConvertListToIList(regexRule.TargetCategoryIds.Where(x => x.IsChecked).Select(x => x.Id.ToString()).ToList()));
            entity.Set("TrackingParameterName", regexRule.TrackingParameterName);
            entity.Set("OutputParameterName", regexRule.OutputParameterName);
            entity.Set("RegexString", regexRule.RegexString);
            entity.Set("MatchTypes", regexRule.MatchTypes.ToString());
            entity.Set("RegexRuleParts", SerializationServices.SerializeRegexRuleParts(regexRule.RegexRuleParts));
            using (Transaction transaction = new Transaction(document, $"Saving RegexRule {regexRule.Name}"))
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
                RegexRule regexRule = RegexRule.Create(documentGuid, entity.Get<Guid>("GUID").ToString());
                regexRule.Name = entity.Get<string>("RuleName");
                List<string> targetTargetCategoryIds = entity.Get<IList<string>>("TargetCategoryIds").ToList();
                regexRule.TrackingParameterName = entity.Get<string>("TrackingParameterName");
                regexRule.OutputParameterName = entity.Get<string>("OutputParameterName");
                regexRule.RegexString = entity.Get<string>("RegexString");
                
                // Deserializing saved match type string to enum value
                switch (entity.Get<string>("MatchTypes"))
                {
                    case "ExactMatch":
                        regexRule.MatchTypes = MatchTypes.ExactMatch;
                        break;
                    case "MatchAtBeginning":
                        regexRule.MatchTypes = MatchTypes.MatchAtBeginning;
                        break;
                    case "PartialMatch":
                        regexRule.MatchTypes = MatchTypes.PartialMatch;
                        break;
                    default:
                        break;
                }

                // Deserializing and creating each rule part from a saved list of strings
                List<string> regexRulePartsString = entity.Get<IList<string>>("RegexRuleParts").ToList();
                regexRule.RegexRuleParts = SerializationServices.DeserializeRegexRulePartsInExtensibleStorage(regexRulePartsString);
                
                ObservableCollection<ObservableObject> observableObjects = ObservableObject.GetInitialCategories(documentGuid);
                foreach(ObservableObject observableObject in observableObjects) { observableObject.IsChecked = targetTargetCategoryIds.Contains(observableObject.Id); }
                regexRule.TargetCategoryIds = observableObjects;
                
                return regexRule;
            }
            Schema regularSchema = GetRegularSchema();

            // Retrieving and testing all DataStorage objects in the document against our Regular schema.
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage.Count < 1) { return null; }

            // Returning any Entities which employ the RegularSchema 
            List<Entity> regexRuleEntities = allDataStorage.Where(x => x.GetEntity(regularSchema).IsValid()).Select(x => x.GetEntity(regularSchema)).ToList();

            ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();
            try
            {
                foreach (Entity entity in regexRuleEntities) { regexRules.Add(ConvertEntityToRegexRule(entity)); }
            }
            catch
            {
                // ignored
            }

            return regexRules;
        }
        public static void UpdateRegexRuleInExtensibleStorage(string documentGuid, string regexRuleGuid, RegexRule newRegexRule)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            KeyValuePair<DataStorage, Entity> ruleInExtensibleStorage = GetRegexRuleInExtensibleStorage(documentGuid, regexRuleGuid);
            DataStorage dataStorage = ruleInExtensibleStorage.Key;
            Entity regexRuleEntity = ruleInExtensibleStorage.Value;

            string previousName = regexRuleEntity.Get<string>("RuleName");

            using (Transaction transaction = new Transaction(document, $"Regular - Modifying Rule {previousName}"))
            {
                transaction.Start();
                regexRuleEntity.Set("RuleName", newRegexRule.Name);
                regexRuleEntity.Set("TargetCategoryIds", SerializationServices.ConvertListToIList(newRegexRule.TargetCategoryIds.Where(x => x.IsChecked).Select(x => x.Id.ToString()).ToList()));
                regexRuleEntity.Set("TrackingParameterName", newRegexRule.TrackingParameterName);
                regexRuleEntity.Set("OutputParameterName", newRegexRule.OutputParameterName);
                regexRuleEntity.Set("RegexString", newRegexRule.RegexString);
                regexRuleEntity.Set("RegexRuleParts", SerializationServices.SerializeRegexRuleParts(newRegexRule.RegexRuleParts));
                regexRuleEntity.Set("MatchTypes", newRegexRule.MatchTypes);
                dataStorage.SetEntity(regexRuleEntity);
                transaction.Commit();
            }
        }
        public static void DeleteRegexRuleFromExtensibleStorage(string documentGuid, string regexRuleGuid)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            KeyValuePair<DataStorage, Entity> ruleInExtensibleStorage = GetRegexRuleInExtensibleStorage(documentGuid, regexRuleGuid);
            DataStorage dataStorage = ruleInExtensibleStorage.Key;
            Entity regexRuleEntity = ruleInExtensibleStorage.Value;

            string ruleName = regexRuleEntity.Get<string>("RuleName");
            using (Transaction transaction = new Transaction(document, $"Regular - Deleting Rule {ruleName}"))
            {
                transaction.Start();
                List<ElementId> dataStorageToDelete = new List<ElementId>() { dataStorage.Id };
                document.Delete(dataStorageToDelete);
                transaction.Commit();
            }
        }
        private static KeyValuePair<DataStorage, Entity> GetRegexRuleInExtensibleStorage(string documentGuid, string regexRuleGuid)
        {
            Document document = DocumentServices.GetRevitDocumentByGuid(documentGuid);
            Schema regularSchema = GetRegularSchema();

            // Retrieving and testing all DataStorage objects in the document against our Regular schema.
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage.Count < 1) { return new KeyValuePair<DataStorage, Entity>(null, null); }

            // Returning any Entities which employ the RegularSchema 
            List<DataStorage> regexRuleDataStorage = allDataStorage.Where(x => x.GetEntity(regularSchema).IsValid()).ToList();
            foreach (DataStorage dataStorage in regexRuleDataStorage)
            {
                Entity regexRuleEntity = dataStorage.GetEntity(regularSchema);
                regexRuleEntity.Get<Guid>("GUID").ToString();
                if (regexRuleEntity.Get<Guid>("GUID").ToString() == regexRuleGuid) return new KeyValuePair<DataStorage, Entity> (dataStorage, regexRuleEntity); 
            }
            return new KeyValuePair<DataStorage, Entity>(null, null);
        }
    }
}
