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

            try
            {
                //We either find or create our schema
                Schema regularSchema = Utilities.ReturnRegularSchema(doc);

                //We return a list of RegexRule objects saved as Entities in our ExtensibleStorage; we'll need to parse these
                List<Entity> regexRuleEntities = ReturnExistingRegexRules(regularSchema);

                //Let's parse those Entities into RegexRules
                ObservableCollection<RegexRule> regexRules = new ObservableCollection<RegexRule>();
                foreach (Entity entity in regexRuleEntities) { regexRules.Add(Utilities.ConvertEntityToRegexRule(_doc, entity)); }

                //The Rule Manager is a modal WPF Window with an IObservableCollection displaying any found RegexRules
                Views.RuleManager ruleManager = new Views.RuleManager(regexRules, doc, app);
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

