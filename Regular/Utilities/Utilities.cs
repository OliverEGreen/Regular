using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Regular.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Regular
{
    public class Utilities
    {
        public static Category FetchCategoryByName(string categoryName, Document doc)
        {
            Categories categoriesList = doc.Settings.Categories;
            foreach (Category category in categoriesList) { if (category.Name == categoryName) return category; }
            return null;
        }

        public static string GetRevitDocumentGuid(Document document) { return document.ProjectInformation.UniqueId; }

        //Useful for returning the Project Parameter we create as our output parameter
        public static Parameter FetchProjectParameterByName(Document doc, string parameterName)
        {
            BindingMap map = doc.ParameterBindings;
            DefinitionBindingMapIterator it = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                string currentParameterName = it.Key.Name;
                if(currentParameterName == parameterName) { return (Parameter)it.Current; }
            }
            return null;
        }

        //This one's the tricky one
        public static Parameter FetchParametersOfCategory(Document doc, Category category)
        {
            return null;
        }

        //Spiderinnet's hacky method to create a project parameter, despite the Revit API's limitations on this
        //From https://spiderinnet.typepad.com/blog/2011/05/parameter-of-revit-api-31-create-project-parameter.html
        //This creates a temporary shared parameters file, a temporary shared parameter
        //It then binds this back to the model as an InstanceBinding and deletes the temporary stuff
        public static Parameter CreateProjectParameter(Document doc, Application app, string parameterName, ParameterType parameterType, CategorySet categorySet, BuiltInParameterGroup builtInParameterGroup, bool isInstanceParameter)
        {
            Transaction transaction = new Transaction(doc, "Regular - Creating New Project Parameter");
            transaction.Start();
            
            string oriFile = app.SharedParametersFilename;
            string tempFile = Path.GetTempFileName() + ".txt";
            using (File.Create(tempFile)) { }
            app.SharedParametersFilename = tempFile;
            ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterName, parameterType);
            ExternalDefinition def = app.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;

            app.SharedParametersFilename = oriFile;
            File.Delete(tempFile);

            Binding binding = app.Create.NewTypeBinding(categorySet);
            if (isInstanceParameter) binding = app.Create.NewInstanceBinding(categorySet);

            BindingMap bindingMap = (new UIApplication(app)).ActiveUIDocument.Document.ParameterBindings;
            bindingMap.Insert(def, binding, builtInParameterGroup);

            transaction.Commit();

            //TaskDialog.Show("Regular", $"New Project Parameter {parameterName} has been created.");
            
            return null;
        }

        public static Category GetCategoryFromBuiltInCategory(Document doc, BuiltInCategory builtInCategory)
        {
            Category category = doc.Settings.Categories.get_Item(builtInCategory);
            if (category != null) return category;
            return null;
        }

        public static CategorySet CreateCategorySetFromListOfCategories(Document doc, Application app, List<Category> categories)
        {
            CategorySet categorySet = app.Create.NewCategorySet();
            for(int i = 0; i < categories.Count; i++) { categorySet.Insert(categories[i]); }
            return categorySet;
        }

        //Helper method to check a model for our schema; indicates whether we've saved anything or not
        public static Schema ReturnRegularSchema(Document _doc)
        {
            Schema regularSchema = null;

            //A method that handles all the faff of constructing the regularschema
            Schema ConstructRegularSchema(Document __doc)
            {
                //Check to see if the schema has already been defined in the document
                if (regularSchema == null)
                {
                    //The schema doesn't exist; we need to define the schema for the first time
                    SchemaBuilder schemaBuilder = new SchemaBuilder(Guid.NewGuid());
                    schemaBuilder.SetSchemaName("RegularSchema");
                    schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                    schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);

                    //Constructing the scheme for rules
                    schemaBuilder.AddSimpleField("ruleName", typeof(string));
                    schemaBuilder.AddSimpleField("categoryName", typeof(string));
                    schemaBuilder.AddSimpleField("trackingParameterName", typeof(string));
                    schemaBuilder.AddSimpleField("outputParameterName", typeof(string));
                    schemaBuilder.AddSimpleField("regexString", typeof(string));
                    schemaBuilder.AddArrayField("regexRuleParts", typeof(string));

                    regularSchema = schemaBuilder.Finish();
                }
                return regularSchema;
            }

            //Collecting all schemas in the model; we want to see if there's a Regular schema amongst them
            IList<Schema> allSchemas = Schema.ListSchemas();

            if (allSchemas != null)
            {
                List<string> allSchemaNames = allSchemas.Select(x => x.SchemaName).ToList();
                if (allSchemaNames.Contains("RegularSchema"))
                {
                    regularSchema = allSchemas.Where(x => x.SchemaName == "RegularSchema").FirstOrDefault();
                    //TaskDialog.Show("Regular - DEMO", "An existing schema for Regular was found in this file. It has been loaded.");
                    //Now we've found there's our valid schema in the model, we'll need to gather the Entities
                    //That employ our schema and load each of them in to display the rule manager page
                    //We'll want to populate these as RegexRule objects into our IObservableCollection
                }
                //There's no regular Schema, we'll need to build it up from scratch.
                else
                {
                    regularSchema = ConstructRegularSchema(_doc);
                    //TaskDialog.Show("Regular - DEMO", "No existing validation rules were found.\nA new Regular schema has been constructed.");
                }
            }
            //There are no schemas in this model, we'll need to build the Regular schema
            else
            {
                regularSchema = ConstructRegularSchema(_doc);
                //TaskDialog.Show("Regular - DEMO", "No existing validation rules were found.\nA new Regular schema has been constructed.");
            }
            return regularSchema;
        }
        
        public static ObservableCollection<RegexRule> ReturnExistingRegexRules(Document _doc, Application _app)
        {
            Schema _regularSchema = ReturnRegularSchema(_doc);

            //Retrieving and testing all DataStorage objects in the document against our Regular schema.
            List<Element> allDataStorageElements = new FilteredElementCollector(_doc).OfClass(typeof(DataStorage)).ToElements().ToList();
            if (allDataStorageElements == null || allDataStorageElements.Count < 1) { return null; }
            List<DataStorage> allDataStorage = allDataStorageElements.Cast<DataStorage>().ToList();

            List<Entity> regexRuleEntities = new List<Entity>();

            foreach (DataStorage dataStorage in allDataStorage)
            {
                Entity entity = dataStorage.GetEntity(_regularSchema);
                if (entity.IsValid()) { regexRuleEntities.Add(entity); }
            }

            ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();
            foreach (Entity entity in regexRuleEntities) { regexRules.Add(ConvertEntityToRegexRule(_doc, entity)); }
            return regexRules;
        }

        public static void SaveRegexRuleToDataStorage(Document doc, Application app)
        {
            return;
        }

        //Helper method to take Entities returned from Storage and convert them to RegexRules (including their RegexRuleParts)
        public static RegexRule ConvertEntityToRegexRule(Document doc, Entity entity)
        {
            string name = entity.Get<string>("ruleName");
            string categoryName = entity.Get<string>("categoryName");
            Category category = FetchCategoryByName(categoryName, doc);
            string trackingParameterName = entity.Get<string>("trackingParameterName");
            //This is tricky. We need to first filter for all parameters that match the category
            //After which we can match them up by the name given by the user in the ComboBox.
            //Unless.... we don't retrieve the parameters themselves and always retrieve strings.
            //Then we just use element.LookupParameter("theName") to read its value when validating?
            //Parameter trackingParameter = FetchProjectParameterByName(doc, trackingParameterName);
            string outputParameterName = entity.Get<string>("outputParameterName");
            //Parameter outputParameter = FetchProjectParameterByName(doc, outputParameterName);
            string regexString = entity.Get<string>("regexString");
            List<string> regexRulePartsString = entity.Get<IList<string>>("regexRuleParts").ToList<string>();

            RegexRule regexRule = new RegexRule(name, category, trackingParameterName, outputParameterName);

            //Helper method to deserialize our smushed-down regex rule parts
            ObservableCollection<RegexRulePart> DeserializeRegexRuleParts(List<string> _regexRulePartsString)
            {
                ObservableCollection<RegexRulePart> _regexRuleParts = new ObservableCollection<RegexRulePart>();

                //Converting RuleParts from serialized strings to real RegexRuleParts
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

                    //Finally, we create the rule part and add to the outgoing list of RegexRuleParts
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
