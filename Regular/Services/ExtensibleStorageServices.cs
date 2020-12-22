using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Regular.Enums;
using Regular.Models;

namespace Regular.Services
{
    public static class ExtensibleStorageServices
    {
        // CRUD Services for managing Regular data stored using Revit's ExtensibleStorage API
        

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
                schemaBuilder.AddArrayField("TargetCategoryObjects", typeof(int));
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
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
            //This needs to be turned into a method taking a RegexRule and saving to ExtensibleStorage
            Entity entity = new Entity(GetRegularSchema());
            entity.Set("GUID", new Guid(regexRule.RuleGuid));
            entity.Set("RuleName", regexRule.RuleName);
            IList<int> targetCategoryIds = SerializationServices.ConvertListToIList
                (
                regexRule.TargetCategoryObjects
                    .Where(x => x.IsChecked)
                    .Select(x => x.CategoryObjectId)
                    .ToList()
                );
            entity.Set("TargetCategoryObjects", targetCategoryIds);
            entity.Set("TrackingParameterName", regexRule.TrackingParameterObject.ParameterObjectName);
            entity.Set("TrackingParameterId", regexRule.TrackingParameterObject.ParameterObjectId);
            entity.Set("OutputParameterName", regexRule.OutputParameterObject.ParameterObjectName);
            entity.Set("OutputParameterId", regexRule.OutputParameterObject.ParameterObjectId);
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
                List<int> targetTargetCategoryIds = entity.Get<IList<int>>("TargetCategoryObjects").ToList();
                ObservableCollection<CategoryObject> categoryObjects = CategoryServices.GetInitialCategories(documentGuid);
                foreach (CategoryObject categoryObject in categoryObjects) { categoryObject.IsChecked = targetTargetCategoryIds.Contains(categoryObject.CategoryObjectId); }
                regexRule.TargetCategoryObjects = categoryObjects;
                regexRule.TrackingParameterObject = new ParameterObject
                {
                    ParameterObjectId = entity.Get<int>("TrackingParameterId"),
                    ParameterObjectName = entity.Get<string>("TrackingParameterName")
                };
                regexRule.OutputParameterObject = new ParameterObject
                {
                    ParameterObjectId = entity.Get<int>("OutputParameterId"),
                    ParameterObjectName = entity.Get<string>("OutputParameterName")
                };
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
                }
                regexRule.IsFrozen = entity.Get<bool>("IsFrozen");
                
                return regexRule;
            }
            Schema regularSchema = GetRegularSchema();

            // Retrieving and testing all DataStorage objects in the document against our Regular schema.
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
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
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
            KeyValuePair<DataStorage, Entity> ruleInExtensibleStorage = GetRegexRuleInExtensibleStorage(documentGuid, regexRuleGuid);
            DataStorage dataStorage = ruleInExtensibleStorage.Key;
            Entity regexRuleEntity = ruleInExtensibleStorage.Value;

            string previousName = regexRuleEntity.Get<string>("RuleName");

            using (Transaction transaction = new Transaction(document, $"Regular - Modifying Rule {previousName}"))
            {
                transaction.Start();
                regexRuleEntity.Set("RuleName", newRegexRule.RuleName);
                IList<int> targetCategoryIds = SerializationServices.ConvertListToIList
                (
                    newRegexRule.TargetCategoryObjects
                        .Where(x => x.IsChecked)
                        .Select(x => x.CategoryObjectId)
                        .ToList()
                );
                regexRuleEntity.Set("TargetCategoryObjects", targetCategoryIds);
                regexRuleEntity.Set("TrackingParameterName", newRegexRule.TrackingParameterObject.ParameterObjectName);
                regexRuleEntity.Set("TrackingParameterId", newRegexRule.TrackingParameterObject.ParameterObjectId);
                regexRuleEntity.Set("OutputParameterName", newRegexRule.OutputParameterObject.ParameterObjectName);
                regexRuleEntity.Set("OutputParameterId", newRegexRule.OutputParameterObject.ParameterObjectId);
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
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
            KeyValuePair<DataStorage, Entity> ruleInExtensibleStorage = GetRegexRuleInExtensibleStorage(documentGuid, regexRuleGuid);
            Entity regexRuleEntity = ruleInExtensibleStorage.Value;
            if (regexRuleEntity == null) return;
            DataStorage dataStorage = ruleInExtensibleStorage.Key;

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
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
            Schema regularSchema = GetRegularSchema();

            // Retrieving and testing all DataStorage objects in the document against our Regular schema.
            List<DataStorage> allDataStorage = new FilteredElementCollector(document).OfClass(typeof(DataStorage)).OfType<DataStorage>().ToList();
            if (allDataStorage.Count < 1) { return new KeyValuePair<DataStorage, Entity>(null, null); }

            // Returning any Entities which employ the RegularSchema 
            List<DataStorage> regexRuleDataStorage = allDataStorage.Where(x => x.GetEntity(regularSchema).IsValid()).ToList();
            foreach (DataStorage dataStorage in regexRuleDataStorage)
            {
                Entity regexRuleEntity = dataStorage.GetEntity(regularSchema);
                if (regexRuleEntity.Get<Guid>("GUID").ToString() == regexRuleGuid) return new KeyValuePair<DataStorage, Entity> (dataStorage, regexRuleEntity); 
            }
            return new KeyValuePair<DataStorage, Entity>(null, null);
        }
    }
}
