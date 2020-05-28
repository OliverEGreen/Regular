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
            entity.Set("TrackingParameterName", ((ComboBoxItem)ComboBoxInputTargetParameter.SelectedItem).Content.ToString());
            entity.Set("OutputParameterName", regexRule.OutputParameterName);
            entity.Set("RegexString", regexRule.RegexString);
            IList<string> regexRulePartList = new List<string>();
            foreach (RegexRulePart regexRulePart in selectedRegexRuleParts)
            {
                regexRulePartList.Add($@"{regexRulePart.RawUserInputValue}:{regexRulePart.RuleType.ToString()}:{regexRulePart.IsOptional.ToString()}");
            }
            entity.Set<IList<string>>("regexRuleParts", regexRulePartList);
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

        // Helper method to take Entities returned from Storage and convert them to RegexRules (including their RegexRuleParts)
        public static RegexRule ConvertEntityToRegexRule(Document doc, Entity entity)
        {
            string name = entity.Get<string>("ruleName");
            string categoryName = entity.Get<string>("categoryName");
            string trackingParameterName = entity.Get<string>("trackingParameterName");
            // This is tricky. We need to first filter for all parameters that match the category
            // After which we can match them up by the name given by the user in the ComboBox.
            // Unless.... we don't retrieve the parameters themselves and always retrieve strings.
            // Then we just use element.LookupParameter("theName") to read its value when validating?
            // Parameter trackingParameter = FetchProjectParameterByName(doc, trackingParameterName);
            string outputParameterName = entity.Get<string>("outputParameterName");
            // Parameter outputParameter = FetchProjectParameterByName(doc, outputParameterName);
            string regexString = entity.Get<string>("regexString");
            List<string> regexRulePartsString = entity.Get<IList<string>>("regexRuleParts").ToList<string>();

            RegexRule regexRule = new RegexRule(name, categoryName, trackingParameterName, outputParameterName);

            // Helper method to deserialize our smushed-down regex rule parts
            ObservableCollection<RegexRulePart> DeserializeRegexRuleParts(List<string> _regexRulePartsString)
            {
                ObservableCollection<RegexRulePart> _regexRuleParts = new ObservableCollection<RegexRulePart>();

                // Converting RuleParts from serialized strings to real RegexRuleParts
                foreach (string serializedString in regexRulePartsString)
                {
                    List<string> serializedStringParts = serializedString.Split(':').ToList();
                    string rawUserInputValue = serializedStringParts[0];
                    string regexRuleTypeString = serializedStringParts[1];
                    string isOptionalString = serializedStringParts[2];
                    RuleTypes ruleType = RuleTypes.None;
                    switch (regexRuleTypeString)
                    {
                        case "AnyLetter":
                            ruleType = RuleTypes.AnyLetter;
                            break;
                        case "SpecificLetter":
                            ruleType = RuleTypes.SpecificLetter;
                            break;
                        case "AnyNumber":
                            ruleType = RuleTypes.AnyNumber;
                            break;
                        case "SpecificNumber":
                            ruleType = RuleTypes.SpecificNumber;
                            break;
                        case "AnyCharacter":
                            ruleType = RuleTypes.AnyCharacter;
                            break;
                        case "SpecificCharacter":
                            ruleType = RuleTypes.SpecificCharacter;
                            break;
                        case "AnyFromSet":
                            ruleType = RuleTypes.AnyFromSet;
                            break;
                        case "Anything":
                            ruleType = RuleTypes.Anything;
                            break;
                        case "Dot":
                            ruleType = RuleTypes.Dot;
                            break;
                        case "Hyphen":
                            ruleType = RuleTypes.Hyphen;
                            break;
                        case "Underscore":
                            ruleType = RuleTypes.Underscore;
                            break;
                    }

                    bool isRulePartOptional = false;
                    if (isOptionalString == "True") { isRulePartOptional = true; }

                    // Finally, we create the rule part and add to the outgoing list of RegexRuleParts
                    RegexRulePart regexRulePart = new RegexRulePart(rawUserInputValue, ruleType, isRulePartOptional);
                    _regexRuleParts.Add(regexRulePart);
                }
                return _regexRuleParts;
            }

            ObservableCollection<RegexRulePart> regexRuleParts = DeserializeRegexRuleParts(regexRulePartsString);
            regexRule.RegexRuleParts = regexRuleParts;
            return regexRule;
        }
    }
}
