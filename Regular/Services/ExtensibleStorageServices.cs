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
        #region All DocumentGUID Code
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

            // Returning the document RuleGuid if the schema is employed, otherwise null
            DataStorage documentGuidDataStorage = allDataStorage.FirstOrDefault(x => x.GetEntity(guidSchema).IsValid());
            Entity documentGuidEntity = documentGuidDataStorage?.GetEntity(guidSchema);
            return documentGuidEntity?.Get<string>("DocumentGUID");
        }
        #endregion

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
                // TODO: Sort out these permissions
                //schemaBuilder.SetReadAccessLevel(AccessLevel.Application);
                //schemaBuilder.SetWriteAccessLevel(AccessLevel.Application);
                //schemaBuilder.SetVendorId("OGRN");
                // Constructing the scheme for regexRules stored in ExtensibleStorage
                schemaBuilder.AddSimpleField("GUID", typeof(Guid));
                schemaBuilder.AddSimpleField("RuleName", typeof(string));
                schemaBuilder.AddArrayField("TargetCategoryIds", typeof(string));
                schemaBuilder.AddSimpleField("TrackingParameterName", typeof(string));
                schemaBuilder.AddSimpleField("TrackingParameterId", typeof(int));
                schemaBuilder.AddSimpleField("OutputParameterName", typeof(string));
                schemaBuilder.AddSimpleField("OutputParameterId", typeof(int));
                schemaBuilder.AddSimpleField("RegexString", typeof(string));
                schemaBuilder.AddArrayField("RegexRuleParts", typeof(string));
                schemaBuilder.AddSimpleField("MatchType", typeof(string));
                schemaBuilder.AddSimpleField("IsFrozen", typeof(bool));
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
            entity.Set("GUID", new Guid(regexRule.RuleGuid));
            entity.Set("RuleName", regexRule.RuleName);
            entity.Set("TargetCategoryIds", SerializationServices.ConvertListToIList(regexRule.TargetCategoryIds.Where(x => x.IsChecked).Select(x => x.Id.ToString()).ToList()));
            entity.Set("TrackingParameterName", regexRule.TrackingParameterName);
            entity.Set("TrackingParameterId", regexRule.TrackingParameterId);
            entity.Set("OutputParameterName", regexRule.OutputParameterName);
            entity.Set("OutputParameterId", regexRule.OutputParameterId);
            entity.Set("RegexString", regexRule.RegexString);
            entity.Set("RegexRuleParts", SerializationServices.SerializeRegexRuleParts(regexRule.RegexRuleParts));
            string matchTypeString = regexRule.MatchType.ToString();
            entity.Set("MatchType", matchTypeString);
            entity.Set("IsFrozen", regexRule.IsFrozen);
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
                RegexRule regexRule = RegexRule.Create(documentGuid, entity.Get<Guid>("GUID").ToString());
                regexRule.RuleName = entity.Get<string>("RuleName");
                List<string> targetTargetCategoryIds = entity.Get<IList<string>>("TargetCategoryIds").ToList();
                ObservableCollection<CategoryObject> observableObjects = CategoryObject.GetInitialCategories(documentGuid);
                foreach (CategoryObject observableObject in observableObjects) { observableObject.IsChecked = targetTargetCategoryIds.Contains(observableObject.Id); }
                regexRule.TargetCategoryIds = observableObjects;
                regexRule.TrackingParameterName = entity.Get<string>("TrackingParameterName");
                regexRule.TrackingParameterId = entity.Get<int>("TrackingParameterId");
                regexRule.OutputParameterName = entity.Get<string>("OutputParameterName");
                regexRule.OutputParameterId = entity.Get<int>("OutputParameterId");
                regexRule.RegexString = entity.Get<string>("RegexString");
                regexRule.RegexRuleParts = DeserializationServices.DeserializeRegexRulePartsInExtensibleStorage(entity.Get<IList<string>>("RegexRuleParts").ToList());
                // Deserializing saved match type string to enum value
                switch (entity.Get<string>("MatchType"))
                {
                    case "ExactMatch":
                        regexRule.MatchType = MatchType.ExactMatch;
                        break;
                    case "MatchAtBeginning":
                        regexRule.MatchType = MatchType.MatchAtBeginning;
                        break;
                    case "PartialMatch":
                        regexRule.MatchType = MatchType.PartialMatch;
                        break;
                    default:
                        break;
                }
                regexRule.IsFrozen = entity.Get<bool>("IsFrozen");
                
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
            catch { }
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
                regexRuleEntity.Set("RuleName", newRegexRule.RuleName);
                regexRuleEntity.Set("TargetCategoryIds", SerializationServices.ConvertListToIList(newRegexRule.TargetCategoryIds.Where(x => x.IsChecked).Select(x => x.Id.ToString()).ToList()));
                regexRuleEntity.Set("TrackingParameterName", newRegexRule.TrackingParameterName);
                regexRuleEntity.Set("TrackingParameterId", newRegexRule.TrackingParameterId);
                regexRuleEntity.Set("OutputParameterName", newRegexRule.OutputParameterName);
                regexRuleEntity.Set("OutputParameterId", newRegexRule.OutputParameterId);
                regexRuleEntity.Set("RegexString", newRegexRule.RegexString);
                regexRuleEntity.Set("RegexRuleParts", SerializationServices.SerializeRegexRuleParts(newRegexRule.RegexRuleParts));
                regexRuleEntity.Set("MatchType", newRegexRule.MatchType.ToString());
                regexRuleEntity.Set("IsFrozen", newRegexRule.IsFrozen);
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
                List<ElementId> dataStorageToDelete = new List<ElementId> { dataStorage.Id };
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
