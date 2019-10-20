using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows;
using System.Linq;
using Autodesk.Revit.DB.ExtensibleStorage;
using Regular.Models;
using Regular;
using System.Collections.ObjectModel;

namespace Regular
{
    [Transaction(TransactionMode.Manual)]
    public class RuleManager : IExternalCommand
    {
        public static Document _doc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application _app { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Setting properties for forms to access document by
            _doc = doc;
            _app = app;

            //Helper method to check a model for our schema; indicates whether we've saved anything or not
            Schema ReturnRegularSchema(Document _doc)
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
                        TaskDialog.Show("Regular", "Yay we found the regular schema all is well.");
                        //Now we've found there's our valid schema in the model, we'll need to gather the Entities
                        //That employ our schema and load each of them in to display the rule manager page
                        //We'll want to populate these as RegexRule objects into our IObservableCollection
                    }
                    //There's no regular Schema, we'll need to build it up from scratch.
                    else
                    {
                        regularSchema = ConstructRegularSchema(doc);
                        TaskDialog.Show("Regular", "We just built the regularschema");
                    }
                }
                //There are no schemas in this model, we'll need to build the Regular schema
                else
                {
                    regularSchema = ConstructRegularSchema(doc);
                    TaskDialog.Show("Regular", "We just built the regularschema");
                }
                return regularSchema;
            }

            //Finds our regex rule entities saved as ExtensibleStorage. We'll need to parse these.
            List<Entity> ReturnExistingRegexRules(Schema regularSchema)
            {
                //This is what we want to return
                List<Entity> validRegexRules = new List<Entity>();

                List<Element> allDataStorageElements = new FilteredElementCollector(doc).OfClass(typeof(DataStorage)).ToElements().ToList();
                if (allDataStorageElements == null) { return null; }
                List<DataStorage> allDataStorage = allDataStorageElements.Cast<DataStorage>().ToList();
                foreach (DataStorage dataStorage in allDataStorage)
                {
                    Entity entity = dataStorage.GetEntity(regularSchema);
                    if (entity.IsValid()) { validRegexRules.Add(entity); }
                }
                return validRegexRules;
            }

            #region Really Dirty String-Based Lookup Methods

            Parameter FetchParameterByName(string parameterName)
            {
                List<Element> projectParameterElements = new FilteredElementCollector(doc).OfClass(typeof(ParameterElement)).ToList();
                List<Parameter> projectParameters = projectParameterElements.Cast<Parameter>().ToList();

                foreach (Parameter parameter in projectParameters)
                {
                    if (parameter.GetType() == typeof(ParameterElement))
                    {
                        if (parameter.Definition.Name == parameterName) { return parameter; }
                    }
                }
                return null;
            }

            #endregion

            //Helper method to take Entities returned from Storage and convert them to RegexRules (including their RegexRuleParts)
            RegexRule ConvertEntityToRegexRule(Entity entity)
            {
                string name = entity.Get<string>("ruleName");
                string categoryName = entity.Get<string>("category");
                Category category = Utilities.FetchCategoryByName(categoryName, doc);
                string trackingParameterName = entity.Get<string>("trackingParameter");
                Parameter trackingParameter = FetchParameterByName(trackingParameterName);
                string outputParameterName = entity.Get<string>("outputParameter");
                Parameter outputParameter = FetchParameterByName(outputParameterName);
                string regexString = entity.Get<string>("regexString");
                List<string> regexRulePartsString = entity.Get<List<string>>("regexRuleParts");

                RegexRule regexRule = new RegexRule(name, category, trackingParameter, outputParameter);

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

            try
            {
                //We either find or create our schema
                Schema regularSchema = ReturnRegularSchema(doc);

                //We return a list of RegexRule objects saved as Entities in our ExtensibleStorage; we'll need to parse these
                List<Entity> regexRuleEntities = ReturnExistingRegexRules(regularSchema);

                //Let's parse those Entities into RegexRules
                ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();
                foreach (Entity entity in regexRuleEntities) { regexRules.Add(ConvertEntityToRegexRule(entity)); }

                //The Rule Manager is a modal WPF Window with an IObservableCollection displaying any found RegexRules
                Regular.Views.RuleManager ruleManager = new Regular.Views.RuleManager(regexRules);
                ruleManager.ShowDialog();
                //We need to build the rule manager UI using IObservableCollection and Listbox. 
                //Need to build the new rule button in order to have ability to create new rules
                //This will involve building a parsing function to parse a new, sort of empty RegexRule into ExtensibleStorage
                //This will involve Entity and DataStorage object stuff, the reverse of before.

                //When the dialog closes we can run all rules in order
                //After rules have run this External Command is over.
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }
    }

}

